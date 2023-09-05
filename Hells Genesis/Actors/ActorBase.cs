using HG.Actors.Weapons;
using HG.Actors.Weapons.Bullets;
using HG.Engine;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HG.Actors
{
    internal class ActorBase
    {
        protected Core _core;
        public bool IsBoostFading { get; set; }

        private Image _image;

        protected Image _lockedOnImage;
        protected Image _lockedOnSoftImage;
        protected AudioClip _hitSound;
        protected AudioClip _shieldHit;
        protected AudioClip _explodeSound;

        protected ActorAnimation _explosionAnimation;
        protected ActorAnimation _hitExplosionAnimation;
        protected ActorAnimation _hitAnimation;

        private SolidBrush _radarDotBrush = new(Color.FromArgb(255, 255, 0, 0));

        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private readonly int _MillisecondsBetweenHits = 200;

        #region Properties.

        public string AssetTag { get; set; }
        public uint UID { get; private set; } = Core.GetNextSequentialId();
        public uint OwnerUID { get; set; }
        public List<ActorAttachment> Attachments { get; private set; } = new();
        public HgPoint<int> RadarDotSize { get; set; } = new HgPoint<int>(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool Highlight { get; set; } = false;
        public HgRotationMode RotationMode { get; set; }
        public int HitPoints { get; private set; } = 0;
        public int ShieldPoints { get; private set; } = 0;

        #region Events.

        public delegate void HitEvent(ActorBase sender, HgDamageType damageType, int damageAmount);
        public event HitEvent OnHit;

        public delegate void QueuedForDeleteEvent(ActorBase sender);
        public event QueuedForDeleteEvent OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(ActorBase sender);
        public event VisibilityChangedEvent OnVisibilityChanged;


        public delegate void ExplodeEvent(ActorBase sender);
        public event ExplodeEvent OnExplode;

        #endregion


        private HgVelocity<double> _velocity;
        public HgVelocity<double> Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
                _velocity.OnThrottleChange += _velocity_OnThrottleChange;
            }
        }

        private void _velocity_OnThrottleChange(HgVelocity<double> sender)
        {
            VelocityChanged();
        }

        public bool IsDead { get; set; } = false;

        public void SetHitPoints(int points)
        {
            HitPoints = 0;
            AddHitPoints(points);
        }

        /// <summary>
        /// Creates a new actor, adds it to the actor collection but also adds it to the collection of another actors children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public ActorAttachment Attach(string imagePath, bool takesDamage = false, int hitPoints = 1)
        {
            var attachment = _core.Actors.AddNewActorAttachment(imagePath, null, UID);
            attachment.TakesDamage = takesDamage;
            attachment.SetHitPoints(hitPoints);
            Attachments.Add(attachment);
            return attachment;
        }

        public void AddHitPoints(int pointsToAdd)
        {
            if (HitPoints + pointsToAdd > _core.Settings.MaxHitpoints)
            {
                pointsToAdd = _core.Settings.MaxHitpoints - (HitPoints + pointsToAdd);
            }
            HitPoints += pointsToAdd;
        }

        public void SetShieldPoints(int points)
        {
            ShieldPoints = 0;
            AddShieldPoints(points);
        }

        public void AddShieldPoints(int pointsToAdd)
        {
            if (ShieldPoints + pointsToAdd > _core.Settings.MaxShieldPoints)
            {
                pointsToAdd = _core.Settings.MaxShieldPoints - (ShieldPoints + pointsToAdd);
            }

            if (this is ActorPlayer)
            {
                var player = this as ActorPlayer;

                if (ShieldPoints < _core.Settings.MaxShieldPoints && ShieldPoints + pointsToAdd >= _core.Settings.MaxShieldPoints)
                {
                    player.ShieldMaxSound.Play();
                }
            }

            ShieldPoints += pointsToAdd;
        }

        bool _isLockedOn = false;
        public bool IsLockedOn //The object is the subject of a foreign weapons lock.
        {
            get
            {
                return _isLockedOn;
            }
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    _core.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        public void QueueForDelete()
        {
            Visable = false;
            if (_readyForDeletion == false)
            {
                VisibilityChanged();
            }

            OnQueuedForDelete?.Invoke(this);

            _readyForDeletion = true;
        }

        private bool _readyForDeletion;
        public bool ReadyForDeletion
        {
            get
            {
                return _readyForDeletion;
            }
        }

        public bool IsOnScreen
        {
            get
            {
                return _core.Display.CurrentScaledScreenBounds.IntersectsWith(Bounds);
            }
        }

        private HgPoint<double> _location = new HgPoint<double>();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public HgPoint<double> Location
        {
            get
            {
                return new HgPoint<double>(_location);
            }
            set
            {
                Invalidate();
                _location = value;
                Invalidate();
            }
        }

        public double X
        {
            get
            {
                return _location.X;
            }
            set
            {
                Invalidate();
                _location.X = value;
                PositionChanged();
                Invalidate();
            }
        }

        public double Y
        {
            get
            {
                return _location.Y;
            }
            set
            {
                Invalidate();
                _location.Y = value;
                PositionChanged();
                Invalidate();
            }
        }

        public HgPoint<double> LocationCenter
        {
            get
            {
                return new HgPoint<double>(_location.X - Size.Width / 2.0, _location.Y - Size.Height / 2.0);
            }
        }

        private Size _size;
        public virtual Size Size
        {
            get
            {
                return _size;
            }
        }

        public RectangleF VisibleBounds
        {
            get
            {
                return new Rectangle((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);
            }
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF((float)_location.X, (float)_location.Y, Size.Width, Size.Height);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)_location.X, (int)_location.Y, Size.Width, Size.Height);
            }
        }

        private bool _isVisible = true;
        public bool Visable
        {
            get
            {
                return _isVisible && !_readyForDeletion;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;

                    OnVisibilityChanged?.Invoke(this);

                    /*
                    var invalidRect = new Rectangle(
                        (int)(_location.X - _size.Width / 2.0),
                        (int)(_location.Y - _size.Height / 2.0),
                        _size.Width, _size.Height);
                    _core.Display.DrawingSurface.Invalidate(invalidRect);
                    */

                    VisibilityChanged();
                }
            }
        }

        #endregion

        public ActorBase(Core core, string assetTag = "")
        {
            _core = core;
            AssetTag = assetTag;
            RotationMode = HgRotationMode.Upsize;
            Velocity = new HgVelocity<double>();
            Velocity.MaxRotationSpeed = _core.Settings.MaxRotationSpeed;
            Highlight = _core.Settings.HighlightAllActors;
        }

        public void Initialize(string imagePath = null, Size? size = null)
        {
            if (imagePath != null)
            {
                SetImage(imagePath, size);
            }

            VisibilityChanged();
        }

        public void ApplyMotion(HgPoint<double> displacementVector)
        {
            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
        }

        public void SetImage(Image image, Size? size = null)
        {
            _image = image;

            if (size != null)
            {
                _image = HgGraphics.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public void SetImage(string imagePath, Size? size = null)
        {
            _image = _core.Imaging.Get(imagePath);

            if (size != null)
            {
                _image = HgGraphics.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }

            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public Image GetImage()
        {
            return _image;
        }

        public void Invalidate()
        {
            /*
            var invalidRect = new Rectangle(
                (int)(_location.X - _size.Width / 2.0),
                (int)(_location.Y - _size.Height / 2.0),
                _size.Width, _size.Height);
            _core.Display.DrawingSurface.Invalidate(invalidRect);
            */
        }

        public bool Intersects(ActorBase otherObject)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        /// <summary>
        /// Allows for intersect detection with larger/smaller "hit box".
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="sizeAdjust"></param>
        /// <returns></returns>
        public bool Intersects(ActorBase otherObject, HgPoint<double> sizeAdjust)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                var alteredHitBox = new RectangleF(
                    otherObject.Bounds.X - (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Y - (float)(sizeAdjust.Y / 2),
                    otherObject.Bounds.Width + (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Height + (float)(sizeAdjust.Y / 2));

                return Bounds.IntersectsWith(alteredHitBox);
            }
            return false;
        }

        public bool Intersects(HgPoint<double> location, HgPoint<double> size)
        {
            var alteredHitBox = new RectangleF(
                (float)location.X,
                (float)location.Y,
                (float)size.X,
                (float)size.Y
                );

            return VisibleBounds.IntersectsWith(alteredHitBox);
        }

        public bool Intersects(ActorBase with, int slop = 0)
        {
            var alteredHitBox = new RectangleF(
                (float)(with.Bounds.X - slop),
                (float)(with.Bounds.Y - slop),
                with.Size.Width + slop * 2, with.Size.Height + slop * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        #region Actions.

        /// <summary>
        /// Subtract from the objects hitpoints.
        /// </summary>
        /// <returns></returns>
        public bool Hit(int damage)
        {
            bool result = (DateTime.Now - _lastHit).TotalMilliseconds > _MillisecondsBetweenHits;
            if (result)
            {
                _lastHit = DateTime.Now;

                if (ShieldPoints > 0)
                {
                    _shieldHit.Play();
                    damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                    damage = damage < 1 ? 1 : damage;
                    damage = damage > ShieldPoints ? ShieldPoints : damage; //No need to go negative with the damage.
                    ShieldPoints -= damage;

                    OnHit?.Invoke(this, HgDamageType.Shield, damage);
                }
                else
                {
                    _hitSound.Play();
                    damage = damage > HitPoints ? HitPoints : damage; //No need to go negative with the damage.
                    HitPoints -= damage;

                    OnHit?.Invoke(this, HgDamageType.Hull, damage);
                }
            }

            return result;
        }

        public bool Hit(BulletBase bullet)
        {
            if (bullet != null)
            {
                return Hit(bullet.Weapon.Damage);
            }
            return false;
        }

        public void Rotate(double degrees)
        {
            Velocity.Angle.Degrees += degrees;
            Invalidate();
            RotationChanged();
        }

        public void MoveInDirectionOf(HgPoint<double> location, double? speed = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, location);
            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(ActorBase obj, double? speed = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, obj.Location);

            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        /// <summary>
        /// Calculated the difference in heading angle from one object to get to another.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        [Obsolete("This function returns the angle in 0-360 degress which is almost impossible to work with, use DeltaAngle() instead.")]
        public double DeltaAngle360(ActorBase atObj)
        {
            return HgMath.DeltaAngle360(this, atObj);
        }

        public double DeltaAngle(ActorBase atObj)
        {
            return HgMath.DeltaAngle(this, atObj);
        }

        /// <summary>
        /// Calculates the angle in degrees of one objects location to another location.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double AngleTo(ActorBase atObj)
        {
            return HgMath.AngleTo(this, atObj);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees)
        {
            return HgMath.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return HgMath.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(ActorBase to)
        {
            return HgPoint<double>.DistanceTo(Location, to.Location);
        }

        public double DistanceTo(HgPoint<double> to)
        {
            return HgPoint<double>.DistanceTo(Location, to);
        }

        public void Explode(bool autoKill = true, bool autoDelete = true)
        {
            foreach (var attachments in Attachments)
            {
                attachments.Explode();
            }

            if (autoKill)
            {
                IsDead = true;
            }

            if (autoDelete)
            {
                QueueForDelete();
            }

            OnExplode?.Invoke(this);
        }

        public void HitExplosion()
        {
            if (_hitExplosionAnimation != null)
            {
                _hitExplosionAnimation.Reset();
                _core.Actors.Animations.CreateAt(_hitExplosionAnimation, this);
            }
        }

        public List<ActorBase> Intersections()
        {
            var intersections = new List<ActorBase>();

            foreach (var intersection in _core.Actors.Collection)
            {
                if (intersection != this && intersection.Visable && intersection is not ActorTextBlock)
                {
                    if (Intersects(intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }

            return intersections;
        }

        public virtual void Cleanup()
        {
            Visable = false;
            Invalidate();

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        public virtual void VelocityChanged()
        {
        }

        public virtual void VisibilityChanged()
        {
        }

        public virtual void PositionChanged()
        {
        }

        public virtual void RotationChanged()
        {
        }

        public void Render(Graphics dc)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(dc, _image);

                if (_lockedOnImage != null && IsLockedOn)
                {
                    DrawImage(dc, _lockedOnImage, 0);
                }
                else if (_lockedOnImage != null && IsLockedOnSoft)
                {
                    DrawImage(dc, _lockedOnSoftImage, 0);
                }

                if (Highlight)
                {
                    using (var pen = new Pen(Color.Red, 3))
                    {
                        var rect = new Rectangle((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);
                        dc.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        public Color RadarDotColor
        {
            set
            {
                _radarDotBrush = new SolidBrush(value);
            }
        }

        public void RenderRadar(Graphics dc, HgPoint<double> scale, HgPoint<double> offset)
        {
            if (_isVisible && _image != null)
            {
                double x = (int)(X * scale.X + offset.X - RadarDotSize.X / 2.0);
                double y = (int)(Y * scale.Y + offset.Y - RadarDotSize.Y / 2.0);

                dc.FillEllipse(_radarDotBrush, (int)x, (int)y, RadarDotSize.X, RadarDotSize.Y);
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            var bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != HgRotationMode.None)
            {
                if (RotationMode == HgRotationMode.Upsize) //Very expensize
                {
                    var image = HgGraphics.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    var rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == HgRotationMode.Clip) //Much less expensive.
                {
                    var image = HgGraphics.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    var rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(_location.X - bitmap.Width / 2.0), (int)(_location.Y - bitmap.Height / 2.0), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        #endregion
    }
}
