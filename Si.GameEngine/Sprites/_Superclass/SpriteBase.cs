using SharpDX.Mathematics.Interop;
using Si.GameEngine.Core;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.Utility;
using Si.Library.ExtensionMethods;
using Si.Library.Payload.SpriteActions;
using Si.Library.Types;
using Si.Library.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites._Superclass
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public class SpriteBase
    {
        #region Multiplayer properties.

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

        #region Backend variables.

        protected GameEngineCore _gameEngine;

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
        private SiPoint _location = new();
        private Size _size;

        #endregion

        #region Properties.

        public string SpriteTag { get; set; }
        public uint UID { get; private set; } = GameEngineCore.GetNextSequentialId();
        public uint OwnerUID { get; set; }
        public List<SpriteAttachment> Attachments { get; private set; } = new();
        public SiPoint RadarDotSize { get; set; } = new SiPoint(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool IsWithinCurrentScaledScreenBounds => _gameEngine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds);
        public bool IsHighlighted { get; set; } = false;
        public int HullHealth { get; private set; } = 0; //Ship hit-points.
        public int ShieldHealth { get; private set; } = 0; //Sheild hit-points, these take 1/2 damage.

        /// <summary>
        /// The sprite still exists, but is not functional (e.g. its been shot and exploded).
        /// </summary>
        public bool IsDeadOrExploded { get; private set; } = false;
        public bool IsQueuedForDeletion => _readyForDeletion;

        /// <summary>
        /// If true, the sprite does not respond to changes in background offset.
        /// </summary>
        public bool IsFixedPosition { get; set; }

        /// <summary>
        /// Width and height of the sprite.
        /// </summary>
        public virtual Size Size => _size;

        /// <summary>
        /// Whether the sprite is rended before speed based scaling.
        /// Note that pre-scaled sprite X,Y is the top, left of the natrual screen bounds.
        /// </summary>
        public SiRenderScaleOrder RenderScaleOrder { get; set; } = SiRenderScaleOrder.PreScale;

        /// <summary>
        /// The bounds of the sprite in the universe.
        /// </summary>
        public virtual RectangleF Bounds => new((float)(Location.X - Size.Width / 2.0), (float)(Location.Y - Size.Height / 2.0), Size.Width, Size.Height);

        /// <summary>
        /// The bounds of the sprite on the display.
        /// </summary>
        public virtual RectangleF RenderBounds => new((float)(RenderLocation.X - Size.Width / 2.0), (float)(RenderLocation.Y - Size.Height / 2.0), Size.Width, Size.Height);

        public SiVelocity Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                _velocity.OnThrottleChanged += (sender) => VelocityChanged();
            }
        }

        public bool IsLockedOn //The object is the subject of a foreign weapons lock.
        {
            get => _isLockedOn;
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    _gameEngine.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        /// <summary>
        /// The x,y, location of the center of the sprite in the universe.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiPoint Location
        {
            get => _location.Clone(); //Changes made to the location object do not affect the sprite.
            set => _location = value;
        }

        /// <summary>
        /// The x,y, location of the center of the sprite on the screen.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiPoint RenderLocation
        {
            get
            {
                if (IsFixedPosition)
                {
                    return _location;
                }
                else
                {
                    return _location - _gameEngine.Display.RenderWindowPosition;
                }
            }
        }

        /// <summary>
        /// The X location of the center of the sprite in the universe.
        /// </summary>
        public double X
        {
            get => _location.X;
            set
            {
                _location.X = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The Y location of the center of the sprite in the universe.
        /// </summary>
        public double Y
        {
            get => _location.Y;
            set
            {
                _location.Y = value;
                LocationChanged();
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

        public SpriteBase(GameEngineCore gameEngine, string name = "")
        {
            _gameEngine = gameEngine;

            IsDrone = GetType().Name.EndsWith("Drone");

            SpriteTag = name;
            Velocity = new SiVelocity();
            IsHighlighted = _gameEngine.Settings.HighlightAllSprites;
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

        public void QueueForDelete()
        {
            if (IsDeadOrExploded == false)
            {
                //This sprite is being deleted but has not been killed. It is likely that this sprite still exists on remote clients.
                _gameEngine.Multiplay.RecordDroneActionDelete(_multiplayUID);
            }

            _readyForDeletion = true;
            Visable = false;
            OnQueuedForDelete?.Invoke(this);
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
                + $"       Ready for Delete?: {IsQueuedForDeletion}\r\n"
                + $"                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"         Render-Location: {RenderLocation}\r\n"
                + $"                Location: {Location}\r\n"
                + $"                   Angle: {Velocity.Angle}\r\n"
                + $"                          {Velocity.Angle.Degrees:n2}deg\r\n"
                + $"                          {Velocity.Angle.Radians:n2}rad\r\n"
                + $"                          {Velocity.Angle.RadiansUnadjusted:n2}rad unadjusted\r\n"
                + extraInfo
                + $"       Background Offset: {_gameEngine.Display.RenderWindowPosition}\r\n"
                + $"                  Thrust: {Velocity.ThrottlePercentage * 100:n2}\r\n"
                + $"                   Boost: {Velocity.BoostPercentage * 100:n2}\r\n"
                + $"                  Recoil: {Velocity.RecoilPercentage * 100:n2}\r\n"
                + $"                    Hull: {HullHealth:n0}\r\n"
                + $"                  Shield: {ShieldHealth:n0}\r\n"
                + $"             Attachments: {Attachments?.Count() ?? 0:n0}\r\n"
                + $"               Highlight: {IsHighlighted}\r\n"
                + $"       Is Fixed Position: {IsFixedPosition}\r\n"
                + $"            Is Locked On: {IsLockedOn}\r\n"
                + $"     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"In Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"          Visible Bounds: {Bounds}\r\n";
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
            var attachment = _gameEngine.Sprites.Attachments.Create(imagePath, null, UID);
            attachment.TakesDamage = takesDamage;
            attachment.SetHullHealth(hullHealth);
            Attachments.Add(attachment);
            return attachment;
        }

        public virtual void AddHullHealth(int pointsToAdd)
        {
            HullHealth += pointsToAdd;
            HullHealth = HullHealth.Box(0, _gameEngine.Settings.MaxHullHealth);
        }

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
        {
            ShieldHealth += pointsToAdd;
            ShieldHealth = ShieldHealth.Box(1, _gameEngine.Settings.MaxShieldPoints);
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
            _image = _gameEngine.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public void SetImage(string imagePath, Size size)
        {
            _image = _gameEngine.Assets.GetBitmap(imagePath, size.Width, size.Height);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        public SharpDX.Direct2D1.Bitmap GetImage() => _image;

        #region Intersections.

        public bool Intersects(SpriteBase otherObject)
        {
            if (Visable && otherObject.Visable && !IsQueuedForDeletion && !otherObject.IsQueuedForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        public bool IntersectsWithTrajectory(SpriteBase otherObject)
        {
            if (Visable && otherObject.Visable)
            {
                var previousPosition = otherObject.Location;

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
            if (Visable && otherObject.Visable && !IsQueuedForDeletion && !otherObject.IsQueuedForDeletion)
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

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Intersect detection with a position using adjusted "hit box" size.
        /// </summary>
        /// <returns></returns>
        public bool RenderLocationIntersects(SiPoint location, SiPoint size)
        {
            var alteredHitBox = new RectangleF(
                (float)location.X,
                (float)location.Y,
                (float)size.X,
                (float)size.Y
                );

            return RenderBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Intersect detection with a position.
        /// </summary>
        /// <returns></returns>
        public bool Intersects(SiPoint location)
        {
            var alteredHitBox = new RectangleF((float)location.X, (float)location.Y, 1f, 1f);
            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Gets a list of all ov this objects intersections.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> Intersections()
        {
            var intersections = new List<SpriteBase>();

            _gameEngine.Sprites.Use(o =>
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
            _gameEngine.Multiplay.RecordDroneActionHit(_multiplayUID);

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
            Velocity.Angle.Degrees = SiPoint.AngleTo360(Location, location);
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
            Velocity.Angle.Degrees = SiPoint.AngleTo360(Location, obj.Location);

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

            if (Velocity.Angle.DegreesNormalized180.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
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

            _gameEngine.Multiplay.RecordDroneActionExplode(_multiplayUID);

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            if (_hitExplosionAnimation != null)
            {
                _hitExplosionAnimation.Reset();
                _gameEngine.Sprites.Animations.AddAt(_hitExplosionAnimation, this);
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

        public double DistanceTo(SpriteBase to) => SiPoint.DistanceTo(Location, to.Location);

        public double DistanceTo(SiPoint to) => SiPoint.DistanceTo(Location, to);

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
                var distance = SiPoint.DistanceTo(Location, to.Location);
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
                var distance = SiPoint.DistanceTo(Location, to.Location);
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
            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
        }

        /// <summary>
        /// Moves the sprite to the exact location as dictated by the remote connection.
        /// Also sets the current vector of the remote sprite so that we can move the sprite along that vector between updates.
        /// </summary>
        /// <param name="vector"></param>
        public virtual void ApplyAbsoluteMultiplayVector(SiSpriteActionVector vector)
        {
            Velocity.ThrottlePercentage = vector.ThrottlePercentage;
            Velocity.BoostPercentage = vector.BoostPercentage;
            Velocity.MaxSpeed = vector.MaxSpeed;
            Velocity.MaxBoost = vector.MaxBoost;
            Velocity.Angle.Degrees = vector.AngleDegrees;
            Velocity.AvailableBoost = 10000; //Just a high number so the drone does not run out of boost.
            X = vector.X;
            Y = vector.Y;
        }

        public virtual SiSpriteActionVector GetMultiplayVector() { return null; }
        public virtual void VelocityChanged() { }
        public virtual void VisibilityChanged() { }
        public virtual void LocationChanged() { }
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

                if (IsHighlighted)
                {
                    var rectangle = new RectangleF((int)(RenderLocation.X - Size.Width / 2.0), (int)(RenderLocation.Y - Size.Height / 2.0), Size.Width, Size.Height);
                    var rawRectF = new RawRectangleF(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
                    _gameEngine.Rendering.DrawRectangleAt(renderTarget, rawRectF, Velocity.Angle.Degrees, _gameEngine.Rendering.Materials.Raw.Red, 0, 1);
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
                    _gameEngine.Rendering.FillTriangleAt(renderTarget, x, y, 3, _gameEngine.Rendering.Materials.Brushes.Orange);
                }
                else if (this is SpriteEnemyBase)
                {
                    _gameEngine.Rendering.FillTriangleAt(renderTarget, x, y, 3, _gameEngine.Rendering.Materials.Brushes.OrangeRed);
                }
                else if (this is MunitionBase)
                {
                    float size;
                    RawColor4 color;

                    var munition = this as MunitionBase;
                    if (munition.FiredFromType == SiFiredFromType.Enemy)
                    {
                        color = _gameEngine.Rendering.Materials.Raw.Red;
                    }
                    else
                    {
                        color = _gameEngine.Rendering.Materials.Raw.Green;
                    }

                    if (munition.Weapon.ExplodesOnImpact)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    _gameEngine.Rendering.FillEllipseAt(renderTarget, x, y, size, size, color);
                }
            }
        }

        private void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, double? angleInDegrees = null)
        {
            float angle = (float)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            _gameEngine.Rendering.DrawBitmapAt(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0,
                RenderLocation.Y - bitmap.Size.Height / 2.0, angle);
        }

        #endregion
    }
}
