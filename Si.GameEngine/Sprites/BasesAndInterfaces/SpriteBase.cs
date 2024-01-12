using SharpDX.Mathematics.Interop;
using Si.GameEngine.Engine;
using Si.GameEngine.Engine.Types;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.ExtensionMethods;
using Si.Shared.Payload.SpriteActions;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public class SpriteBase
    {
        protected EngineCore _gameCore;

        #region Multiplay.

        public SiControlledBy ControlledBy { get; set; }

        public bool IsDrone { get; set; }

        /// <summary>
        /// Each connected client has a sprite with the same matching UID.
        /// </summary>
        private Guid? _multiplayUID = null;

        public Guid MultiplayUID
        {
            get
            {
                if (_multiplayUID == null)
                {
                    if (IsDrone)
                    {
                        throw new Exception("Drones can not be the originators of MultiplayUIDs.");
                    }
                    _multiplayUID = Guid.NewGuid();
                }
                return (Guid)_multiplayUID;
            }
            set
            {
                if (IsDrone == false)
                {
                    throw new Exception("Only drones can have their MultiplayUIDs set.");
                }
                if (_multiplayUID != null & _multiplayUID != Guid.Empty)
                {
                    throw new Exception("MultiplayUIDs can not be set twice.");
                }
                _multiplayUID = value;
                if (_multiplayUID == null || _multiplayUID == Guid.Empty)
                {
                    throw new Exception("Drone MultiplayUIDs can not be NULL or empty.");
                }
            }
        }

        #endregion

        private SharpDX.Direct2D1.Bitmap _image;

        protected SharpDX.Direct2D1.Bitmap _lockedOnImage;
        protected SharpDX.Direct2D1.Bitmap _lockedOnSoftImage;
        protected SiAudioClip _hitSound;
        protected SiAudioClip _shieldHit;
        protected SiAudioClip _explodeSound;

        protected SpriteAnimation _explosionAnimation;
        protected SpriteAnimation _hitExplosionAnimation;
        protected SpriteAnimation _hitAnimation;

        private bool _isLockedOn = false;
        private SiVelocity _velocity;
        private bool _readyForDeletion;
        private SiPoint _localLocation = new();
        private SiPoint _multiPlayLocation = new();
        private Size _size;

        #region Properties.

        public string SpriteTag { get; set; }
        public uint UID { get; private set; } = EngineCore.GetNextSequentialId();
        public uint OwnerUID { get; set; }
        public List<SpriteAttachment> Attachments { get; private set; } = new();
        public SiPoint RadarDotSize { get; set; } = new SiPoint(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool IsWithinCurrentScaledScreenBounds => _gameCore.Display.GetCurrentScaledScreenBounds().IntersectsWith(Bounds);
        public bool Highlight { get; set; } = false;
        public SiRotationMode RotationMode { get; set; }
        public int HullHealth { get; private set; } = 0; //Ship hit-points.
        public int ShieldHealth { get; private set; } = 0; //Sheild hit-points, these take 1/2 damage.

        /// <summary>
        /// The sprite still exists, but is not functional (e.g. its been shot and exploded).
        /// </summary>
        public bool IsDeadOrExploded { get; private set; } = false;
        public bool QueuedForDeletion => _readyForDeletion;
        public bool IsFixedPosition { get; set; }
        public virtual Size Size => _size;
        public SiPoint LocationCenter => new((_localLocation.X + _multiPlayLocation.X) - Size.Width / 2.0, (_localLocation.Y + _multiPlayLocation.Y) - Size.Height / 2.0);
        public RectangleF VisibleBounds => new Rectangle((int)((_localLocation.X + _multiPlayLocation.X) - Size.Width / 2.0), (int)((_localLocation.Y + _multiPlayLocation.Y) - Size.Height / 2.0), Size.Width, Size.Height);
        public RectangleF Bounds => new((float)(_localLocation.X + _multiPlayLocation.X), (float)(_localLocation.Y + _multiPlayLocation.Y), Size.Width, Size.Height);
        public Rectangle BoundsI => new((int)(_localLocation.X + _multiPlayLocation.X), (int)(_localLocation.Y + _multiPlayLocation.Y), Size.Width, Size.Height);
        public SiQuadrant Quadrant => _gameCore.Display.GetQuadrant(LocalX + _gameCore.Display.BackgroundOffset.X, LocalY + _gameCore.Display.BackgroundOffset.Y);

        public SiVelocity Velocity
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
            string extraInfo = string.Empty;

            if (this is SpriteEnemyBase enemy)
            {
                extraInfo =
                      $"           AI Controller: {enemy.CurrentAIController}\r\n"
                    + $"              Is Hostile: {enemy.IsHostile}\r\n";
            }

            return
                  $">                    UID: {UID}\r\n"
                + $"           Multiplay UID: {MultiplayUID}\r\n"
                + $"               Owner UID: {OwnerUID:n0}\r\n"
                + $"                    Name: {GetType().Name}\r\n"
                + $"                     Tag: {SpriteTag:n0}\r\n"
                + $"             Is Visable?: {Visable:n0}\r\n"
                + $"                    Size: {Size:n0}\r\n"
                + $"                  Bounds: {Bounds:n0}\r\n"
                + $"       Ready for Delete?: {QueuedForDeletion}\r\n"
                + $"                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"           Real Location: {RealLocation}\r\n"
                + $"          Local-Location: {LocalLocation}\r\n"
                + $"      Multiplay-Location: {MultiplayLocation}\r\n"
                + $"        Virtual Location: {VirtualLocation}\r\n"
                + $"                   Angle: {Velocity.Angle}\r\n"
                + $"                          {Velocity.Angle.Degrees:n2}deg\r\n"
                + $"                          {Velocity.Angle.Radians:n2}rad\r\n"
                + $"                          {Velocity.Angle.RadiansUnadjusted:n2}rad unadjusted\r\n"
                + extraInfo
                + $"       Background Offset: {_gameCore.Display.BackgroundOffset}\r\n"
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
            var attachment = _gameCore.Sprites.Attachments.Create(imagePath, null, UID);
            attachment.TakesDamage = takesDamage;
            attachment.SetHullHealth(hullHealth);
            Attachments.Add(attachment);
            return attachment;
        }

        public virtual void AddHullHealth(int pointsToAdd)
        {
            HullHealth += pointsToAdd;
            HullHealth = HullHealth.Box(0, _gameCore.Settings.MaxHullHealth);
        }

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
        {
            ShieldHealth += pointsToAdd;
            ShieldHealth = ShieldHealth.Box(1, _gameCore.Settings.MaxShieldPoints);
        }

        public bool IsLockedOn //The object is the subject of a foreign weapons lock.
        {
            get => _isLockedOn;
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    _gameCore.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        public void QueueForDelete()
        {
            if (IsDeadOrExploded == false)
            {
                //This sprite is being deleted but has not been killed. It is likely that this sprite still exists on remote clients.
                _gameCore.Multiplay.RecordDroneActionDelete(_multiplayUID);
            }

            _readyForDeletion = true;
            Visable = false;
            OnQueuedForDelete?.Invoke(this);
        }

        /// <summary>
        /// The location as dictated by a remote connection (when this sptire is a multiplay drone).
        /// Returns the location as a 2d point. Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiPoint MultiplayLocation
        {
            get => new SiPoint(_multiPlayLocation, true);
            set => _multiPlayLocation = value.ToWriteableCopy();
        }

        /// <summary>
        /// The combined local and multiplay location of the sptire.
        /// Returns the location as a 2d point. Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiPoint RealLocation
        {
            get => new SiPoint(_localLocation.X + _multiPlayLocation.X, _localLocation.Y + _multiPlayLocation.Y, true);
        }

        /// <summary>
        /// The local client client location.
        /// Returns the location as a 2d point. Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiPoint LocalLocation
        {
            get => new SiPoint(_localLocation, true);
            set => _localLocation = value.ToWriteableCopy();
        }

        public SiPoint VirtualLocation
        {
            get => new SiPoint(RealLocation.X + _gameCore.Display.BackgroundOffset.X, RealLocation.Y + _gameCore.Display.BackgroundOffset.Y);
        }

        /// <summary>
        /// Typically speaking, this would match the LocalX at the client that owns the sprite.
        /// </summary>
        public double MultiplayX
        {
            get => _multiPlayLocation.X;
            set
            {
                _multiPlayLocation.X = value;
                PositionChanged();
            }
        }

        /// <summary>
        /// Typically speaking, this would match the LocalY at the client that owns the sprite.
        /// </summary>
        public double MultiplayY
        {
            get => _multiPlayLocation.Y;
            set
            {
                _multiPlayLocation.Y = value;
                PositionChanged();
            }
        }

        public double LocalX
        {
            get => _localLocation.X;
            set
            {
                _localLocation.X = value;
                PositionChanged();
            }
        }

        public double LocalY
        {
            get => _localLocation.Y;
            set
            {
                _localLocation.Y = value;
                PositionChanged();
            }
        }

        private bool _isVisible = true;
        public bool Visable
        {
            get => _isVisible && !_readyForDeletion;
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

        public delegate void HitEvent(SpriteBase sender, SiDamageType damageType, int damageAmount);
        public event HitEvent OnHit;

        public delegate void QueuedForDeleteEvent(SpriteBase sender);
        public event QueuedForDeleteEvent OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(SpriteBase sender);
        public event VisibilityChangedEvent OnVisibilityChanged;


        public delegate void ExplodeEvent(SpriteBase sender);
        public event ExplodeEvent OnExplode;

        #endregion

        public SpriteBase(EngineCore gameCore, string name = "")
        {
            _gameCore = gameCore;

            IsDrone = GetType().Name.EndsWith("Drone");

            SpriteTag = name;
            RotationMode = SiRotationMode.Rotate;
            Velocity = new SiVelocity();
            Highlight = _gameCore.Settings.HighlightAllSprites;
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

        public void ReviveDeadOrExploded()
        {
            IsDeadOrExploded = false;
        }

        public void SetImage(SharpDX.Direct2D1.Bitmap bitmap)
        {
            _image = bitmap;
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath)
        {
            _image = _gameCore.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath, Size size)
        {
            _image = _gameCore.Assets.GetBitmap(imagePath, size.Width, size.Height);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public SharpDX.Direct2D1.Bitmap GetImage() => _image;

        #region Intersections.

        public bool Intersects(SpriteBase otherObject)
        {
            if (Visable && otherObject.Visable && !QueuedForDeletion && !otherObject.QueuedForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        public bool IntersectsWithTrajectory(SpriteBase otherObject)
        {
            if (Visable && otherObject.Visable)
            {
                var previousPosition = otherObject.RealLocation.ToWriteableCopy();

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
        public bool Intersects(SpriteBase otherObject, SiPoint sizeAdjust)
        {
            if (Visable && otherObject.Visable && !QueuedForDeletion && !otherObject.QueuedForDeletion)
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
        public bool Intersects(SpriteBase with, int variance = 0)
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
        public bool Intersects(SiPoint location, SiPoint size)
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
        public bool Intersects(SiPoint location)
        {
            var alteredHitBox = new RectangleF((float)location.X, (float)location.Y, 1f, 1f);
            return VisibleBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Gets a list of all ov this objects intersections.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> Intersections()
        {
            var intersections = new List<SpriteBase>();

            _gameCore.Sprites.Use(o =>
            {
                foreach (var intersection in o)
                {
                    if (intersection != this && intersection.Visable && intersection is not SpriteTextBlock)
                    {
                        if (Intersects(intersection))
                        {
                            intersections.Add(intersection);
                        }
                    }
                }
            });

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
            _gameCore.Multiplay.RecordDroneActionHit(_multiplayUID);

            if (ShieldHealth > 0)
            {
                _shieldHit.Play();
                damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                damage = damage < 1 ? 1 : damage;
                damage = damage > ShieldHealth ? ShieldHealth : damage; //No need to go negative with the damage.
                ShieldHealth -= damage;

                OnHit?.Invoke(this, SiDamageType.Shield, damage);
            }
            else
            {
                _hitSound.Play();
                damage = damage > HullHealth ? HullHealth : damage; //No need to go negative with the damage.
                HullHealth -= damage;

                OnHit?.Invoke(this, SiDamageType.Hull, damage);
            }
        }

        /// <summary>
        /// Hits this object with a given munition.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(MunitionBase munition)
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
        public void PointAtAndGoto(SiPoint location, double? velocity = null)
        {
            Velocity.Angle.Degrees = SiPoint.AngleTo360(RealLocation, location);
            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Instantly points an object at another object and sets the travel speed. Only used for off-screen transitions.
        /// </summary>
        public void PointAtAndGoto(SpriteBase obj, double? velocity = null)
        {
            Velocity.Angle.Degrees = SiPoint.AngleTo360(RealLocation, obj.RealLocation);

            if (velocity != null)
            {
                Velocity.MaxSpeed = (double)velocity;
            }
        }

        /// <summary>
        /// Rotates the object towards the target object by the specified amount.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object it not in the specifid range.</returns>
        public bool RotateTo(SpriteBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateTo(SpriteBase obj, SiRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(obj);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == SiRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == SiRelativeDirection.Left)
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
        public bool RotateTo(SiPoint toLocation, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateTo(SiPoint toLocation, SiRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = DeltaAngle(toLocation);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == SiRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == SiRelativeDirection.Left)
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
        public bool RotateTo(double toDegrees, SiRelativeDirection direction, double rotationAmount = 1, double tolerance = 10)
        {
            toDegrees = toDegrees.DegreesNormalized();

            if (Velocity.Angle.DegreesNormalized.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                if (direction == SiRelativeDirection.Right)
                {
                    Velocity.Angle.Degrees += rotationAmount;
                }
                if (direction == SiRelativeDirection.Left)
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
        public bool RotateFrom(SpriteBase obj, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
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
        public bool RotateFrom(SpriteBase obj, SiRelativeDirection direction, double rotationAmount = 1, double untilPointingAtDegreesFallsBetween = 10)
        {
            var deltaAngle = obj.DeltaAngle(this);

            if (deltaAngle.IsBetween(-untilPointingAtDegreesFallsBetween, untilPointingAtDegreesFallsBetween) == false)
            {
                if (direction == SiRelativeDirection.Right)
                {
                    Velocity.Angle += rotationAmount;
                }
                if (direction == SiRelativeDirection.Left)
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

            IsDeadOrExploded = true;
            _isVisible = false;

            if (this is not SpriteAttachment) //Attachments are deleted when the owning object is deleted.
            {
                QueueForDelete();
            }

            _gameCore.Multiplay.RecordDroneActionExplode(_multiplayUID);

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            if (_hitExplosionAnimation != null)
            {
                _hitExplosionAnimation.Reset();
                _gameCore.Sprites.Animations.AddAt(_hitExplosionAnimation, this);
            }
        }

        #endregion

        #region Sprite geometry.

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <returns></returns>
        public double DeltaAngle(SpriteBase toObj) => SiMath.DeltaAngle(this, toObj);

        /// <summary>
        /// Calculates the difference in heading angle from one object to get to another between 1-180 and -1-180
        /// </summary>
        /// <=>s></returns>
        public double DeltaAngle(SiPoint toLocation) => SiMath.DeltaAngle(this, toLocation);

        /// <summary>
        /// Calculates the angle in degrees to another object between 0-259.
        /// </summary>
        /// <returns></returns>
        public double AngleTo360(SpriteBase atObj) => SiMath.AngleTo360(this, atObj);

        /// <summary>
        /// Calculates the angle in degrees to another object between 1-180 and -1-180
        /// </summary>
        /// <returns></returns>
        public double AngleTo(SpriteBase atObj) => SiMath.AngleTo(this, atObj);

        /// <summary>
        /// Calculates the angle in degrees to a location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        [Obsolete("This method is deprecated. Use AngleTo() instead.")]
        public double AngleTo360(SiPoint location) => SiMath.AngleTo360(this, location);

        public bool IsPointingAt(SpriteBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
            => SiMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);

        public bool IsPointingAt(SpriteBase atObj, double toleranceDegrees, double maxDistance) => SiMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);

        public bool IsPointingAt(SpriteBase atObj, double toleranceDegrees) => SiMath.IsPointingAt(this, atObj, toleranceDegrees);

        /// <summary>
        /// Returns true if any of the given sprites are pointing at this one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atObjs"></param>
        /// <param name="toleranceDegrees"></param>
        /// <returns></returns>
        public bool IsPointingAtAny<T>(List<T> atObjs, double toleranceDegrees) where T : SpriteBase
        {
            foreach (var atObj in atObjs)
            {
                if (SiMath.IsPointingAt(this, atObj, toleranceDegrees))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// From the list of given sprites, returns the list of sprites that are pointing at us.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atObjs"></param>
        /// <param name="toleranceDegrees"></param>
        /// <returns></returns>
        public List<T> GetPointingAtOf<T>(List<T> atObjs, double toleranceDegrees) where T : SpriteBase
        {
            var results = new List<T>();

            foreach (var atObj in atObjs)
            {
                if (SiMath.IsPointingAt(this, atObj, toleranceDegrees))
                {
                    results.Add(atObj);
                }
            }
            return results;
        }

        public bool IsPointingAway(SpriteBase atObj, double toleranceDegrees) => SiMath.IsPointingAway(this, atObj, toleranceDegrees);

        public bool IsPointingAway(SpriteBase atObj, double toleranceDegrees, double maxDistance) => SiMath.IsPointingAway(this, atObj, toleranceDegrees, maxDistance);

        public double DistanceTo(SpriteBase to) => SiPoint.DistanceTo(RealLocation, to.RealLocation);

        public double DistanceTo(SiPoint to) => SiPoint.DistanceTo(RealLocation, to);

        /// <summary>
        /// Of the given sprites, returns the sprite that is the closest.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tos"></param>
        /// <returns></returns>
        public T ClosestOf<T>(List<T> tos) where T : SpriteBase
        {
            double closestDistance = double.MaxValue;
            T closestSprite = tos.First();

            foreach (var to in tos)
            {
                var distance = SiPoint.DistanceTo(RealLocation, to.RealLocation);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSprite = to;
                };
            }

            return closestSprite;
        }

        /// <summary>
        /// Of the given sprites, returns the distance of the closest one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tos"></param>
        /// <returns></returns>
        public double ClosestDistanceOf<T>(List<T> tos) where T : SpriteBase
        {
            double closestDistance = double.MaxValue;

            foreach (var to in tos)
            {
                var distance = SiPoint.DistanceTo(RealLocation, to.RealLocation);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                };
            }

            return closestDistance;
        }

        #endregion

        /// <summary>
        /// Moves the sprite based on its thrust/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(SiPoint displacementVector)
        {
            if (IsFixedPosition == false)
            {
                LocalX += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
                LocalY += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
            }
        }

        /// <summary>
        /// Moves the sprite to the exact location as dictated by the remote connection.
        /// Also sets the current vector of the remote sprite so that we can move the sprite along that vector between updates.
        /// </summary>
        /// <param name="vector"></param>
        public virtual void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector)
        {
            /* This is handled in SpriteEnemyBase and SpritePlayerBase.
            Velocity.ThrottlePercentage = vector.ThrottlePercentage;
            Velocity.BoostPercentage = vector.BoostPercentage;
            Velocity.MaxSpeed = vector.MaxSpeed;
            Velocity.MaxBoost = vector.MaxBoost;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            Velocity.AvailableBoost = 10000; //Just a high number so the drone does not run out of boost.

            MultiplayX = vector.X;
            MultiplayY = vector.Y;
            */
        }

        public virtual SiSpriteActionVector GetMultiplayVector() { return null; }
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
                    var rectangle = new RectangleF((int)((_localLocation.X + _multiPlayLocation.X) - Size.Width / 2.0), (int)((_localLocation.Y + _multiPlayLocation.Y) - Size.Height / 2.0), Size.Width, Size.Height);
                    var rawRectF = new RawRectangleF(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
                    _gameCore.Rendering.DrawRectangleAt(renderTarget, rawRectF, Velocity.Angle.Degrees, _gameCore.Rendering.Materials.Raw.Red, 0, 1);
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
                if (this is SpritePlayerBase player && player.IsDrone)
                {
                    _gameCore.Rendering.FillTriangleAt(renderTarget, x, y, 3, _gameCore.Rendering.Materials.Brushes.Orange);
                }
                else if (this is SpriteEnemyBase)
                {
                    _gameCore.Rendering.FillTriangleAt(renderTarget, x, y, 3, _gameCore.Rendering.Materials.Brushes.OrangeRed);
                }
                else if (this is MunitionBase)
                {
                    float size;
                    RawColor4 color;

                    var munition = this as MunitionBase;
                    if (munition.FiredFromType == SiFiredFromType.Enemy)
                    {
                        color = _gameCore.Rendering.Materials.Raw.Red;
                    }
                    else
                    {
                        color = _gameCore.Rendering.Materials.Raw.Green;
                    }

                    if (munition.Weapon.ExplodesOnImpact)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    _gameCore.Rendering.FillEllipseAt(renderTarget, x, y, size, size, color);
                }
            }
        }

        private void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double? angleInDegrees = null)
        {
            float angle = (float)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            if (RotationMode != SiRotationMode.None)
            {
                _gameCore.Rendering.DrawBitmapAt(renderTarget, bitmap,
                    (_localLocation.X + _multiPlayLocation.X) - bitmap.Size.Width / 2.0,
                    (_localLocation.Y + _multiPlayLocation.Y) - bitmap.Size.Height / 2.0, angle);
            }
            else //Almost free.
            {
                _gameCore.Rendering.DrawBitmapAt(renderTarget, bitmap,
                    (_localLocation.X + _multiPlayLocation.X) - bitmap.Size.Width / 2.0,
                    (_localLocation.Y + +_multiPlayLocation.Y) - bitmap.Size.Height / 2.0);
            }
        }

        #endregion
    }
}
