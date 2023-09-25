using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites.Enemies;
using NebulaSiege.Utility;
using NebulaSiege.Utility.ExtensionMethods;
using NebulaSiege.Weapons.Munitions;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NebulaSiege.Sprites
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    internal class _SpriteBase
    {
        protected EngineCore _core;

        private SharpDX.Direct2D1.Bitmap _image;

        protected SharpDX.Direct2D1.Bitmap _lockedOnImage;
        protected SharpDX.Direct2D1.Bitmap _lockedOnSoftImage;
        protected NsAudioClip _hitSound;
        protected NsAudioClip _shieldHit;
        protected NsAudioClip _explodeSound;

        protected SpriteAnimation _explosionAnimation;
        protected SpriteAnimation _hitExplosionAnimation;
        protected SpriteAnimation _hitAnimation;

        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        //private readonly int _MillisecondsBetweenHits = 200;

        private bool _isLockedOn = false;
        private HgVelocity _velocity;
        private bool _readyForDeletion;
        private NsPoint _location = new();
        private Size _size;

        #region Properties.

        public string SpriteTag { get; set; }
        public uint UID { get; private set; } = EngineCore.GetNextSequentialId();
        public uint OwnerUID { get; set; }
        public List<SpriteAttachment> Attachments { get; private set; } = new();
        public NsPoint RadarDotSize { get; set; } = new NsPoint(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool IsWithinCurrentScaledScreenBounds => _core.Display.GetCurrentScaledScreenBounds().IntersectsWith(Bounds);
        public bool Highlight { get; set; } = false;
        public HgRotationMode RotationMode { get; set; }
        public int HullHealth { get; private set; } = 0; //Ship hit-points.
        public int ShieldHealth { get; private set; } = 0; //Sheild hit-points, these take 1/2 damage.
        public bool IsDead { get; set; } = false;
        public bool ReadyForDeletion => _readyForDeletion;
        public bool IsFixedPosition { get; set; }
        public virtual Size Size => _size;
        public NsPoint LocationCenter => new(_location.X - Size.Width / 2.0, _location.Y - Size.Height / 2.0);
        public RectangleF VisibleBounds => new Rectangle((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);
        public RectangleF Bounds => new((float)_location.X, (float)_location.Y, Size.Width, Size.Height);
        public Rectangle BoundsI => new((int)_location.X, (int)_location.Y, Size.Width, Size.Height);
        public HgQuadrant Quadrant => _core.Display.GetQuadrant(X + _core.Display.BackgroundOffset.X, Y + _core.Display.BackgroundOffset.Y);

        public HgVelocity Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                _velocity.OnThrottleChanged += (sender) => VelocityChanged();
            }
        }

        public string GetInspectionText()
        {
            return
                  $">                    UID: {UID}y\r\n"
                + $"                   Owner: {OwnerUID:n0}\r\n"
                + $"                     Tag: {SpriteTag:n0}\r\n"
                + $"             Is Visable?: {Visable:n0}\r\n"
                + $"                    Size: {Size:n0}\r\n"
                + $"                  Bounds: {Bounds:n0}\r\n"
                + $"       Ready for Delete?: {ReadyForDeletion}\r\n"
                + $"                Is Dead?: {IsDead}\r\n"
                + $"                Location: {Location}\r\n"
                + $"                   Angle: {Velocity.Angle}y\r\n"
                + $"                          {Velocity.Angle.Degrees:n2}deg\r\n"
                + $"                          {Velocity.Angle.Radians:n2}rad\r\n"
                + $"                          {Velocity.Angle.RadiansUnadjusted:n2}rad unadjusted\r\n"
                + $"              Virtual XY: X:{X + _core.Display.BackgroundOffset.X:n0}, Y:{Y + _core.Display.BackgroundOffset.Y:n0}\r\n"
                + $"       Background Offset: {_core.Display.BackgroundOffset}\r\n"
                + $"                  Thrust: {(Velocity.ThrottlePercentage * 100):n2}\r\n"
                + $"                   Boost: {(Velocity.BoostPercentage * 100):n2}\r\n"
                + $"                  Recoil: {(Velocity.RecoilPercentage * 100):n2}\r\n"
                + $"                    Hull: {HullHealth:n0}\r\n"
                + $"                  Shield: {ShieldHealth:n0}\r\n"
                + $"                Rotation: {RotationMode}\r\n"
                + $"             Attachments: {(Attachments?.Count() ?? 0):n0}\r\n"
                + $"               Highlight: {Highlight}\r\n"
                + $"       Is Fixed Position: {IsFixedPosition}\r\n"
                + $"            Is Locked On: {IsLockedOn}\r\n"
                + $"     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"In Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"               Quandrant: {Quadrant}\r\n"
                + $"          Visible Bounds: {VisibleBounds}\r\n";
        }

        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment Attach(string imagePath, bool takesDamage = false, int hullHealth = 1)
        {
            var attachment = _core.Sprites.Attachments.Create(imagePath, null, UID);
            attachment.TakesDamage = takesDamage;
            attachment.SetHullHealth(hullHealth);
            Attachments.Add(attachment);
            return attachment;
        }

        public virtual void AddHullHealth(int pointsToAdd)
        {
            HullHealth += pointsToAdd;
            HullHealth = HullHealth.Box(0, _core.Settings.MaxHullHealth);
        }

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
        {
            ShieldHealth += pointsToAdd;
            ShieldHealth = ShieldHealth.Box(1, _core.Settings.MaxShieldPoints);
        }

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

        /// <summary>
        /// Returns the location as a 2d point. Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public NsPoint Location
        {
            get
            {
                return new NsPoint(_location, true);
            }
            set
            {
                _location = value.ToWriteableCopy();
            }
        }

        public void SetLocation(NsPoint location)
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

        public delegate void HitEvent(_SpriteBase sender, HgDamageType damageType, int damageAmount);
        public event HitEvent OnHit;

        public delegate void QueuedForDeleteEvent(_SpriteBase sender);
        public event QueuedForDeleteEvent OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(_SpriteBase sender);
        public event VisibilityChangedEvent OnVisibilityChanged;


        public delegate void ExplodeEvent(_SpriteBase sender);
        public event ExplodeEvent OnExplode;

        #endregion

        public _SpriteBase(EngineCore core, string name = "")
        {
            _core = core;
            SpriteTag = name;
            RotationMode = HgRotationMode.Rotate;
            Velocity = new HgVelocity();
            Highlight = _core.Settings.HighlightAllSprites;
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
            _image = _core.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath, Size size)
        {
            _image = _core.Assets.GetBitmap(imagePath, size.Width, size.Height);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public SharpDX.Direct2D1.Bitmap GetImage()
        {
            return _image;
        }

        #region Intersections.

        public bool Intersects(_SpriteBase otherObject)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        public bool IntersectsWithTrajectory(_SpriteBase otherObject)
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
        public bool Intersects(_SpriteBase otherObject, NsPoint sizeAdjust)
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
        public bool Intersects(_SpriteBase with, int variance = 0)
        {
            var alteredHitBox = new RectangleF(
                (float)(with.Bounds.X - variance),
                (float)(with.Bounds.Y - variance),
                with.Size.Width + variance * 2, with.Size.Height + variance * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Intersect detection with a position using adjusted "hit box" size.
        /// </summary>
        /// <returns></returns>
        public bool Intersects(NsPoint location, NsPoint size)
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
        public bool Intersects(NsPoint location)
        {
            var alteredHitBox = new RectangleF((float)location.X, (float)location.Y, 1f, 1f);
            return VisibleBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Gets a list of all ov this objects intersections.
        /// </summary>
        /// <returns></returns>
        public List<_SpriteBase> Intersections()
        {
            var intersections = new List<_SpriteBase>();

            foreach (var intersection in _core.Sprites.Collection)
            {
                if (intersection != this && intersection.Visable && intersection is not SpriteTextBlock)
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
        /// Subtract from the objects hullHealth.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(int damage)
        {
            _lastHit = DateTime.Now;

            if (ShieldHealth > 0)
            {
                _shieldHit.Play();
                damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                damage = damage < 1 ? 1 : damage;
                damage = damage > ShieldHealth ? ShieldHealth : damage; //No need to go negative with the damage.
                ShieldHealth -= damage;

                OnHit?.Invoke(this, HgDamageType.Shield, damage);
            }
            else
            {
                _hitSound.Play();
                damage = damage > HullHealth ? HullHealth : damage; //No need to go negative with the damage.
                HullHealth -= damage;

                OnHit?.Invoke(this, HgDamageType.Hull, damage);
            }
        }

        /// <summary>
        /// Hits this object with a given munition.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(_MunitionBase munition)
        {
            Hit(munition?.Weapon?.Damage ?? 0);
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
        public void PointAtAndGoto(NsPoint location, double? velocity = null)
        {
            Velocity.Angle.Degrees = NsPoint.AngleTo(Location, location);
            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Instantly points an object at another object and sets the travel speed. Only used for off-screen transitions.
        /// </summary>
        public void PointAtAndGoto(_SpriteBase obj, double? velocity = null)
        {
            Velocity.Angle.Degrees = NsPoint.AngleTo(Location, obj.Location);

            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Rotates the object towards the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(_SpriteBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateTo(_SpriteBase obj, HgRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(obj);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == HgRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == HgRelativeDirection.Left)
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
        public bool RotateTo(NsPoint toLocation, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateTo(NsPoint toLocation, HgRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(toLocation);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == HgRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == HgRelativeDirection.Left)
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
        public bool RotateTo(double toDegrees, HgRelativeDirection direction, double rotationAmount = 1, double tolerance = 10)
        {
            toDegrees = toDegrees.DegreesNormalized();

            if (Velocity.Angle.DegreesNormalized.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                if (direction == HgRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == HgRelativeDirection.Left)
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
        public bool RotateFrom(_SpriteBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateFrom(_SpriteBase obj, HgRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = obj.DeltaAngle(this);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == HgRelativeDirection.Right)
                {
                    Velocity.Angle += rotationAmount;
                }
                if (direction == HgRelativeDirection.Left)
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

            if (this is not SpriteAttachment) //Attachments are deleted when the owning object is deleted.
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
                _core.Sprites.Animations.InsertAt(_hitExplosionAnimation, this);
            }
        }

        #endregion

        #region Sprite geometry.

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 0-259.
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle360(_SpriteBase toObj) => HgMath.DeltaAngle360(this, toObj);

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle(_SpriteBase toObj) => HgMath.DeltaAngle(this, toObj);

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <=>s></returns>
        public double DeltaAngle(NsPoint toLocation) => HgMath.DeltaAngle(this, toLocation);

        /// <summary>
        /// Calculates the angle in degrees to another object,
        /// </summary>
        /// <returns></returns>
        public double AngleTo(_SpriteBase atObj) => HgMath.AngleTo(this, atObj);

        /// Calculates the angle in degrees to a location.
        public double AngleTo(NsPoint location) => HgMath.AngleTo(this, location);

        public bool IsPointingAt(_SpriteBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
            => HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);

        public bool IsPointingAt(_SpriteBase atObj, double toleranceDegrees, double maxDistance) => HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);

        public bool IsPointingAt(_SpriteBase atObj, double toleranceDegrees) => HgMath.IsPointingAt(this, atObj, toleranceDegrees);

        public bool IsPointingAway(_SpriteBase atObj, double toleranceDegrees) => HgMath.IsPointingAway(this, atObj, toleranceDegrees);

        public bool IsPointingAway(_SpriteBase atObj, double toleranceDegrees, double maxDistance) => HgMath.IsPointingAway(this, atObj, toleranceDegrees, maxDistance);

        public double DistanceTo(_SpriteBase to) => NsPoint.DistanceTo(Location, to.Location);

        public double DistanceTo(NsPoint to) => NsPoint.DistanceTo(Location, to);

        #endregion

        public virtual void ApplyMotion(NsPoint displacementVector)
        {
            if (IsFixedPosition == false)
            {
                X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
                Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
            }
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

                    _core.DirectX.DrawRectangleAt(renderTarget, rectangle.ToRawRectangleF(), Velocity.Angle.Degrees, _core.DirectX.Materials.Raw.Red, 0, 1);
                }
            }
        }

        public virtual void Render(Graphics dc)
        {
        }

        public void RenderRadar(SharpDX.Direct2D1.RenderTarget renderTarget, int x, int y)
        {
            if (_isVisible && _image != null)
            {
                if (this is _SpriteEnemyBase)
                {
                    _core.DirectX.FillTriangleAt(renderTarget, x, y, 3, _core.DirectX.Materials.Brushes.WhiteSmoke);
                }
                else if (this is _MunitionBase)
                {
                    float size;

                    RawColor4 color = _core.DirectX.Materials.Raw.Blue;

                    var munition = this as _MunitionBase;
                    if (munition.FiredFromType == HgFiredFromType.Enemy)
                    {
                        color = _core.DirectX.Materials.Raw.Red;
                    }
                    else
                    {
                        color = _core.DirectX.Materials.Raw.Green;
                    }

                    if (munition.Weapon.ExplodesOnImpact)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    _core.DirectX.FillEllipseAt(renderTarget, x, y, size, size, color);

                }

            }
        }

        private void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double? angleInDegrees = null)
        {
            float angle = (float)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            if (RotationMode != HgRotationMode.None)
            {
                _core.DirectX.DrawBitmapAt(renderTarget, bitmap,
                    _location.X - bitmap.Size.Width / 2.0,
                    _location.Y - bitmap.Size.Height / 2.0, angle);
            }
            else //Almost free.
            {
                _core.DirectX.DrawBitmapAt(renderTarget, bitmap, _location.X - bitmap.Size.Width / 2.0, _location.Y - bitmap.Size.Height / 2.0);
            }
        }

        #endregion
    }
}
