using HG.Actors.Ordinary;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Utility.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using static HG.Engine.Constants;

namespace HG.Actors.BaseClasses
{
    internal class ActorBase
    {
        protected Core _core;

        private SharpDX.Direct2D1.Bitmap _image;

        protected SharpDX.Direct2D1.Bitmap _lockedOnImage;
        protected SharpDX.Direct2D1.Bitmap _lockedOnSoftImage;
        protected AudioClip _hitSound;
        protected AudioClip _shieldHit;
        protected AudioClip _explodeSound;

        protected ActorAnimation _explosionAnimation;
        protected ActorAnimation _hitExplosionAnimation;
        protected ActorAnimation _hitAnimation;

        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private readonly int _MillisecondsBetweenHits = 200;

        #region Properties.

        public string Name { get; set; }
        public uint UID { get; private set; } = Core.GetNextSequentialId();
        public uint OwnerUID { get; set; }
        public List<ActorAttachment> Attachments { get; private set; } = new();
        public HgPoint<int> RadarDotSize { get; set; } = new HgPoint<int>(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool Highlight { get; set; } = false;
        public HgRotationMode RotationMode { get; set; }
        public int HitPoints { get; private set; } = 0;
        public int ShieldPoints { get; private set; } = 0;

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
        /// <returns></returns>
        public ActorAttachment Attach(string imagePath, bool takesDamage = false, int hitPoints = 1)
        {
            var attachment = _core.Actors.Attachments.Create(imagePath, null, UID);
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
        /// Returns the location as a 2d point. Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public HgPoint<double> Location
        {
            get
            {
                return new HgPoint<double>(_location, true);
            }
            set
            {
                _location = value.ToWriteableCopy();
            }
        }

        public void SetLocation(HgPoint<double> location)
        {
            _location = location;
            PositionChanged();
        }

        public double X
        {
            get
            {
                return _location.X;
            }
            set
            {
                _location.X = value;
                PositionChanged();
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
                _location.Y = value;
                PositionChanged();
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
                    VisibilityChanged();
                }
            }
        }

        #endregion

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

        public ActorBase(Core core, string name = "")
        {
            _core = core;
            Name = name;
            RotationMode = HgRotationMode.Rotate;
            Velocity = new HgVelocity<double>();
            Velocity.MaxRotationSpeed = _core.Settings.MaxRotationSpeed;
            Highlight = _core.Settings.HighlightAllActors;
        }

        public virtual void Initialize(string imagePath = null, Size? size = null)
        {
            if (imagePath != null)
            {
                if (size == null)
                {
                    SetImage(imagePath);
                }
                else
                {
                    SetImage(imagePath, (Size)size);
                }
            }

            VisibilityChanged();
        }

        public void SetImage(SharpDX.Direct2D1.Bitmap bitmap)
        {
            _image = bitmap;
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = _core.Imaging.Get(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath, Size size)
        {
            _image = _core.Imaging.Get(imagePath, size.Width, size.Height);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public SharpDX.Direct2D1.Bitmap GetImage()
        {
            return _image;
        }

        #region Intersections.

        public bool Intersects(ActorBase otherObject)
        {

            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        public bool IntersectsWithTrajectory(ActorBase otherObject)
        {
            if (Visable && otherObject.Visable)
            {
                var previousPosition = otherObject.Location.ToWriteableCopy();

                for (int i = 0; i < otherObject.Velocity.MaxSpeed; i++)
                {
                    previousPosition.X -= otherObject.Velocity.Angle.X;
                    previousPosition.Y -= otherObject.Velocity.Angle.Y;

                    if (Intersects(previousPosition))
                    {
                        return true;

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Intersect detection with another object using adjusted "hit box" size.
        /// </summary>
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

        /// <summary>
        /// Intersect detection with another object using adjusted "hit box" size.
        /// </summary>
        /// <returns></returns>
        public bool Intersects(ActorBase with, int slop = 0)
        {
            var alteredHitBox = new RectangleF(
                (float)(with.Bounds.X - slop),
                (float)(with.Bounds.Y - slop),
                with.Size.Width + slop * 2, with.Size.Height + slop * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Intersect detection with a position using adjusted "hit box" size.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Intersect detection with a position.
        /// </summary>
        /// <returns></returns>
        public bool Intersects(HgPoint<double> location)
        {
            var alteredHitBox = new RectangleF((float)location.X, (float)location.Y, 1f, 1f);

            return VisibleBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Gets a list of all ov this objects intersections.
        /// </summary>
        /// <returns></returns>
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

        #endregion

        #region Actions.

        /// <summary>
        /// Subtract from the objects hitpoints.
        /// </summary>
        /// <returns></returns>
        public virtual bool Hit(int damage)
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

        /// <summary>
        /// Hits this object with a given bullet.
        /// </summary>
        /// <returns></returns>
        public virtual bool Hit(BulletBase bullet)
        {
            if (bullet != null)
            {
                return Hit(bullet.Weapon.Damage);
            }
            return false;
        }

        /// <summary>
        /// Instantly rotates this object by a given degrees.
        /// </summary>
        public void Rotate(double degrees)
        {
            Velocity.Angle.Degrees += degrees;
            RotationChanged();
        }

        /// <summary>
        /// Instantly points an object at a location and sets the travel speed. Only used for off-screen transitions.
        /// </summary>
        public void PointAtAndGoto(HgPoint<double> location, double? velocity = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, location);
            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Instantly points an object at another object and sets the travel speed. Only used for off-screen transitions.
        /// </summary>
        public void PointAtAndGoto(ActorBase obj, double? velocity = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, obj.Location);

            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Rotates the object towards the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(ActorBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(obj);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (deltaAngle >= -untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                else if (deltaAngle < untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle.Degrees -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object towards the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(ActorBase obj, RelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(obj);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == RelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == RelativeDirection.Left)
                {
                    Velocity.Angle.Degrees -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object towards the target coordinates by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(HgPoint<double> toLocation, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(toLocation);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (deltaAngle >= -untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                else if (deltaAngle < untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle.Degrees -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object towards the target coordinates by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(HgPoint<double> toLocation, RelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(toLocation);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == RelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == RelativeDirection.Left)
                {
                    Velocity.Angle.Degrees -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object by the specified amount until it is pointing at the target angle (with given tolerance).
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(double toDegrees, RelativeDirection direction, double rotationAmount = 1, double tolerance = 10)
        {
            toDegrees = toDegrees.DegreesNormalized();

            if (Velocity.Angle.DegreesNormalized.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                if (direction == RelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == RelativeDirection.Left)
                {
                    Velocity.Angle.Degrees -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object from the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateFrom(ActorBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = obj.DeltaAngle(this);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (deltaAngle >= -untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle += rotationAmount;
                }
                else if (deltaAngle < untilPointingAtDegreesFallsBetween)
                {
                    Velocity.Angle -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the object from the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateFrom(ActorBase obj, RelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = obj.DeltaAngle(this);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == RelativeDirection.Right)
                {
                    Velocity.Angle += rotationAmount;
                }
                if (direction == RelativeDirection.Left)
                {
                    Velocity.Angle -= rotationAmount;
                }
                return true;
            }

            return false;
        }

        public virtual void Explode()
        {
            foreach (var attachments in Attachments)
            {
                attachments.Explode();
            }

            IsDead = true;
            _isVisible = false;

            if (this is not ActorAttachment) //Attachments are deleted when the owning object is deleted.
            {
                QueueForDelete();
            }

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            if (_hitExplosionAnimation != null)
            {
                _hitExplosionAnimation.Reset();
                _core.Actors.Animations.CreateAt(_hitExplosionAnimation, this);
            }
        }

        #endregion

        #region Actor geometry.

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 0-259.
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle360(ActorBase toObj)
        {
            return HgMath.DeltaAngle360(this, toObj);
        }

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle(ActorBase toObj)
        {
            return HgMath.DeltaAngle(this, toObj);
        }

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle(HgPoint<double> toLocation)
        {
            return HgMath.DeltaAngle(this, toLocation);
        }

        /// <summary>
        /// Calculates the angle in degrees to another object,
        /// </summary>
        /// <returns></returns>
        public double AngleTo(ActorBase atObj)
        {
            return HgMath.AngleTo(this, atObj);
        }

        /// Calculates the angle in degrees to a location.
        public double AngleTo(HgPoint<double> location)
        {
            return HgMath.AngleTo(this, location);
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


        #endregion

        public virtual void ApplyMotion(HgPoint<double> displacementVector)
        {
            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
        }

        public virtual void VelocityChanged() { }
        public virtual void VisibilityChanged() { }
        public virtual void PositionChanged() { }
        public virtual void RotationChanged() { }

        public virtual void Cleanup()
        {
            Visable = false;

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        #region Rendering.

        public virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(renderTarget, _image);

                if (_lockedOnImage != null && IsLockedOn)
                {
                    DrawImage(renderTarget, _lockedOnImage, 0);
                }
                else if (_lockedOnImage != null && IsLockedOnSoft)
                {
                    DrawImage(renderTarget, _lockedOnSoftImage, 0);
                }

                if (Highlight)
                {
                    var rectangle = new RectangleF((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);

                    _core.DirectX.DrawRectangleAt(renderTarget, rectangle.ToRawRectangleF(), (float)this.Velocity.Angle.Degrees, _core.DirectX.Colors.Raw.Red, 0, 1);
                }
            }
        }

        public virtual void Render(Graphics dc)
        {
        }

        public void RenderRadar(SharpDX.Direct2D1.RenderTarget renderTarget, HgPoint<double> scale, HgPoint<double> offset)
        {
            if (_isVisible && _image != null)
            {
                _core.DirectX.FillEllipseAt(renderTarget,
                    (float)(offset.X + (X * scale.X)),
                    (float)(offset.Y + (Y * scale.Y)),
                    2, //RadiusX
                    2, //RadiusY
                    _core.DirectX.Colors.Raw.Red);
            }
        }

        private void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double? angleInDegrees = null)
        {
            float angle = (float)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            if (RotationMode != HgRotationMode.None)
            {
                _core.DirectX.DrawBitmapAt(renderTarget, bitmap, (int)(_location.X - bitmap.Size.Width / 2.0), (int)(_location.Y - bitmap.Size.Height / 2.0), angle);
            }
            else //Almost free.
            {
                _core.DirectX.DrawBitmapAt(renderTarget, bitmap, (int)(_location.X - bitmap.Size.Width / 2.0), (int)(_location.Y - bitmap.Size.Height / 2.0));
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
        }

        #endregion
    }
}
