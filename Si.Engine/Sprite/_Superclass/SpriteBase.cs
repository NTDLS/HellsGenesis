using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Sprite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public class SpriteBase : ISprite
    {
        #region Backend variables.

        protected EngineCore _engine;

        private SharpDX.Direct2D1.Bitmap _image;
        private bool _readyForDeletion;
        private SiVector _location = new();
        private Size _size;

        #endregion

        #region Travel Vector.

        /// <summary>
        /// The speed that this object can generally travel in any direction.
        /// </summary>
        public float Speed { get; set; }

        private SiVector _movementVector = new();
        /// <summary>
        /// Omni-directional velocity.
        /// </summary>
        public SiVector MovementVector
        {
            get => _movementVector;
            set
            {
                _movementVector = value;
                VelocityChanged();
            }
        }

        private float _throttle = 1.0f;
        /// <summary>
        /// Percentage of speed expressed as a decimal percentage from 0.0 (stopped) to 1000.0 (1000x the normal speed).
        /// </summary>
        public float Throttle
        {
            get => _throttle;
            set => _throttle = value.Clamp(0, 1000);
        }

        /// <summary>
        /// The general maximum throttle that can be applied. This can be considered the "boost" speed.
        /// </summary>
        public float MaxThrottle { get; set; }

        #endregion

        #region Properties.

        /// <summary>
        /// Number or radians to rotate the sprite Orientation along its center at each call to ApplyMotion().
        /// Negative for counter-clockwise, positive for clockwise.
        /// </summary>
        public float RotationSpeed { get; set; } = 0;

        /// <summary>
        /// The angle in which the sprite is pointing, note that this is NOT the travel angle.
        /// The travel angle is baked into the MovementVector. If you need the movement vector
        /// to follow this direction angle then call RecalculateMovementVector() after modifying
        /// the PointingAngle.
        /// </summary>
        public SiVector Orientation { get; set; } = new();

        public SharpDX.Direct2D1.Bitmap GetImage() => _image;
        public string SpriteTag { get; set; }
        public uint UID { get; private set; } = SiSequenceGenerator.Next();
        public uint OwnerUID { get; set; }
        public SiVector LocationRelativeToOwner { get; set; }
        public List<SpriteAttachment> Attachments { get; private set; } = new();
        public SiVector RadarDotSize { get; set; } = new SiVector(4, 4);
        public bool IsWithinCurrentScaledScreenBounds => _engine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds);
        public bool IsHighlighted { get; set; } = false;
        public int HullHealth { get; private set; } = 0; //Ship hit-points.
        public int ShieldHealth { get; private set; } = 0; //Shield hit-points, these take 1/2 damage.

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

        public int ZOrder { get; set; } = 0;

        /// <summary>
        /// The bounds of the sprite in the universe.
        /// </summary>
        public virtual RectangleF Bounds => new(
                (Location.X - Size.Width / 2.0f),
                (Location.Y - Size.Height / 2.0f),
                Size.Width,
                Size.Height);

        /// <summary>
        /// The raw bounds of the sprite in the universe.
        /// </summary>
        public virtual RawRectangleF RawBounds => new(
                        (Location.X - Size.Width / 2.0f),
                        (Location.Y - Size.Height / 2.0f),
                        (Location.X - Size.Width / 2.0f) + Size.Width,
                        (Location.Y - Size.Height / 2.0f) + Size.Height);

        /// <summary>
        /// The bounds of the sprite on the display.
        /// </summary>
        public virtual RectangleF RenderBounds => new(
                        (RenderLocation.X - Size.Width / 2.0f),
                        (RenderLocation.Y - Size.Height / 2.0f),
                        Size.Width,
                        Size.Height);

        /// <summary>
        /// The raw bounds of the sprite on the display.
        /// </summary>
        public virtual RawRectangleF RawRenderBounds => new(
                        (RenderLocation.X - Size.Width / 2.0f),
                        (RenderLocation.Y - Size.Height / 2.0f),
                        (RenderLocation.X - Size.Width / 2.0f) + Size.Width,
                        (RenderLocation.Y - Size.Height / 2.0f) + Size.Height);


        /// <summary>
        /// The x,y, location of the center of the sprite in the universe.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiVector Location
        {
            get => _location.Clone(); //Changes made to the location object do not affect the sprite.
            set
            {
                _location = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The top left corner of the sprite in the universe.
        /// </summary>
        public SiVector LocationTopLeft
        {
            get => _location - (Size / 2.0f); //Changes made to the location object do not affect the sprite.
            set
            {
                _location = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The x,y, location of the center of the sprite on the screen.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public SiVector RenderLocation
        {
            get
            {
                if (IsFixedPosition)
                {
                    return _location;
                }
                else
                {
                    return _location - _engine.Display.RenderWindowPosition;
                }
            }
        }

        /// <summary>
        /// The X location of the center of the sprite in the universe.
        /// </summary>
        public float X
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
        public float Y
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

        public SpriteBase(EngineCore engine, string spriteTag = "")
        {
            _engine = engine;

            SpriteTag = spriteTag;
            IsHighlighted = _engine.Settings.HighlightAllSprites;
        }

        /// <summary>
        /// Sets the movement vector in the direction of the sprite taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector() => MovementVector = MakeMovementVector();

        /// <summary>
        /// Sets the movement vector in the given direction taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(float angleInRadians) => MovementVector = MakeMovementVector(angleInRadians);

        /// <summary>
        /// Sets the movement vector in the given direction taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(SiVector angle) => MovementVector = MakeMovementVector(angle);

        /// <summary>
        /// Returns the movement vector in the direction of the sprite taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector() => Orientation * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(float angleInRadians) => new SiVector(angleInRadians) * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into accoun the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(SiVector angle) => angle.Normalize() * Speed * Throttle;

        public void QueueForDelete()
        {
            _readyForDeletion = true;
            Visable = false;
            OnQueuedForDelete?.Invoke(this);
        }

        /// <summary>
        /// Sets the sprites center to the center of the screen.
        /// </summary>
        public void CenterInUniverse()
        {
            X = _engine.Display.TotalCanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = _engine.Display.TotalCanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        #region Post-movement movement vector collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite made on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> FindReverseCollisionsAlongMovementVector(float epoch)
            => FindReverseCollisionsAlongMovementVector(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns a list of all collisions the sprite made on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public List<SpriteBase> FindReverseCollisionsAlongMovementVector(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object after it has been moved and tests each location
            ///     betwwen where it ended up and where it should have come from given its movement vector.

            var collisions = new List<SpriteBase>();

            //Get the starting position of the sprite before it was last moved.
            var hitTestPosition = new SiVector(Location - (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(hitTestPosition));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns the first collision (if any) the sprite made on is current movement vector.
        /// </summary>
        /// <returns></returns>
        public SpriteBase FindFirstReverseCollisionAlongMovementVectorAABB(float epoch)
            => FindFirstReverseCollisionAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns the first collision (if any) the sprite made on is current movement vector.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public SpriteBase FindFirstReverseCollisionAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object after it has been moved and tests each location
            ///     betwwen where it ended up and where it should have come from given its movement vector.

            //Get the starting position of the sprite before it was last moved.
            var hitTestPosition = new SiVector(Location - (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(hitTestPosition));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Pre-movement movement vector collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite will make on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <returns></returns>
        public List<SpriteBase> FindForwardCollisionsAlongMovementVectorAABB(float epoch)
            => FindForwardCollisionsAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns a list of all collisions the sprite will make on is current movement vector, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public List<SpriteBase> FindForwardCollisionsAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object before it has been moved and tests each location
            ///     betwwen where it is and where it will end up given its movement vector.

            var collisions = new List<SpriteBase>();

            var hitTestPosition = new SiVector(Location);
            var destinationPoint = new SiVector(Location + (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(destinationPoint));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns the first collision (if any) the sprite will make on is current movement vector.
        /// </summary>
        /// <returns></returns>
        public SpriteBase FindFirstForwardCollisionAlongMovementVectorAABB(float epoch)
            => FindFirstForwardCollisionAlongMovementVectorAABB(_engine.Sprites.Visible(), epoch);

        /// <summary>
        /// Returns the first collision (if any) the sprite will make on is current movement vector.
        /// </summary>
        /// <param name="objectsThatCanBeHit"></param>
        /// <returns></returns>
        public SpriteBase FindFirstForwardCollisionAlongMovementVectorAABB(SpriteBase[] objectsThatCanBeHit, float epoch)
        {
            /// Takes the position of an object before it has been moved and tests each location
            ///     betwwen where it is and where it will end up given its movement vector.

            var hitTestPosition = new SiVector(Location);
            var destinationPoint = new SiVector(Location + (MovementVector * epoch));
            var directionVector = MovementVector.Normalize();
            var totalTravelDistance = Math.Abs(Location.DistanceTo(destinationPoint));

            if (totalTravelDistance > _engine.Display.TotalCanvasDiagonal)
            {
                //This is just a sanity check, if the epoch is super high then the engine is
                //  lagging like mad and the last thing we want is to trace a giant vector path.
                // Keep in mind that we are tracing the individual steps per "frame", so this IS NOT
                //  going to greatly effect collision detection even if the lagging is really bad.
                totalTravelDistance = _engine.Display.TotalCanvasDiagonal;
            }

            //Hit-test each position along the sprite path.
            for (int i = 0; i < totalTravelDistance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        #region Vector distance collision detection.

        /// <summary>
        /// Returns a list of all collisions the sprite will make over a given distance and optional angle, in the order in which they would be encountered.
        /// </summary>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public List<SpriteBase> FindCollisionsAlongDistanceVectorAABB(float distance, SiVector angle = null)
            => FindCollisionsAlongDistanceVectorAABB(_engine.Sprites.Visible(), distance, angle);

        /// <summary>
        ///  Returns a list of all collisions the sprite will make over a given distance and optional angle, in the order in which they would be encountered.
        /// </summary>
        /// <param name="objectsThatCanBeHit">List of objects to test for collisions.</param>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public List<SpriteBase> FindCollisionsAlongDistanceVectorAABB(SpriteBase[] objectsThatCanBeHit, float distance, SiVector angle = null)
        {
            var collisions = new List<SpriteBase>();

            var hitTestPosition = new SiVector(Location);
            var directionVector = angle ?? Orientation;

            //Hit-test each position along the sprite path.
            for (int i = 0; i < distance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        collisions.Add(obj);
                    }
                }
            }

            return collisions;
        }

        /// <summary>
        /// Returns a the first object the sprite will collide with over a given distance and optional angle.
        /// </summary>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public SpriteBase FindFirstCollisionAlongDistanceVectorAABB(float distance, SiVector angle = null)
            => FindFirstCollisionAlongDistanceVectorAABB(_engine.Sprites.Visible(), distance, angle);

        /// <summary>
        /// Returns a the first object the sprite will collide with over a given distance and optional angle.
        /// </summary>
        /// <param name="objectsThatCanBeHit">List of objects to test for collisions.</param>
        /// <param name="distance">Distance to detect collisions.</param>
        /// <param name="angle">Optional angle for detection, if not specified then the sprites forward angle is used.</param>
        /// <returns></returns>
        public SpriteBase FindFirstCollisionAlongDistanceVectorAABB(SpriteBase[] objectsThatCanBeHit, float distance, SiVector angle = null)
        {
            var hitTestPosition = new SiVector(Location);
            var directionVector = angle ?? Orientation;

            //Hit-test each position along the sprite path.
            for (int i = 0; i < distance; i++)
            {
                hitTestPosition += directionVector;
                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.IntersectsAABB(hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Allows for the testing of hits from a munition, 
        /// </summary>
        /// <param name="munition">The munition object that is being tested for.</param>
        /// <param name="hitTestPosition">The position to test for hit.</param>
        /// <returns></returns>
        public virtual bool TryMunitionHit(MunitionBase munition, SiVector hitTestPosition)
        {
            if (IntersectsAABB(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }


        public virtual void MunitionHit(MunitionBase munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                Explode();
            }
        }

        public string GetInspectionText()
        {
            string extraInfo = string.Empty;

            if (this is SpriteEnemyBase enemy)
            {
                extraInfo =
                      $"           AI Controller: {enemy.CurrentAIController}\r\n";
            }

            return
                  $">                    UID: {UID}\r\n"
                + $"               Owner UID: {OwnerUID:n0}\r\n"
                + $"                    Type: {GetType().Name}\r\n"
                + $"                     Tag: {SpriteTag:n0}\r\n"
                + $"             Is Visable?: {Visable:n0}\r\n"
                + $"                    Size: {Size:n0}\r\n"
                + $"                  Bounds: {Bounds:n0}\r\n"
                + $"       Ready for Delete?: {IsQueuedForDeletion}\r\n"
                + $"                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"         Render-Location: {RenderLocation}\r\n"
                + $"                Location: {Location}\r\n"
                + $"                   Angle: {Orientation}\r\n"
                + $"                          {Orientation.DegreesSigned:n2}deg\r\n"
                + $"                          {Orientation.RadiansSigned:n2}rad\r\n"
                + extraInfo
                + $"       Background Offset: {_engine.Display.RenderWindowPosition}\r\n"
                + $"                  Thrust: {MovementVector * 100:n2}\r\n"
                + $"                   Boost: {Throttle * 100:n2}\r\n"
                + $"                    Hull: {HullHealth:n0}\r\n"
                + $"                  Shield: {ShieldHealth:n0}\r\n"
                + $"             Attachments: {Attachments?.Count ?? 0:n0}\r\n"
                + $"               Highlight: {IsHighlighted}\r\n"
                + $"       Is Fixed Position: {IsFixedPosition}\r\n"
                //+ $"            Is Locked On: {IsLockedOnHard}\r\n"
                //+ $"     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"In Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"          Visible Bounds: {Bounds}\r\n";
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment Attach(string imagePath)
        {
            var attachment = _engine.Sprites.Attachments.Add(this, imagePath);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment Attach<T>(string imagePath) where T : SpriteAttachment
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf<T>(this, imagePath);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment AttachOfType<T>() where T : SpriteAttachment
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf<T>(this);
            Attachments.Add(attachment);
            return attachment;
        }

        /// <summary>
        /// Creates a new sprite, adds it to the sprite collection but also adds it to the collection of another sprites children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <returns></returns>
        public SpriteAttachment AttachOfType(string typeName, SiVector locationRelativeToOwner)
        {
            var attachment = _engine.Sprites.Attachments.AddTypeOf(typeName, this, locationRelativeToOwner);
            Attachments.Add(attachment);
            return attachment;
        }

        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        public virtual void AddHullHealth(int pointsToAdd)
        {
            HullHealth = (HullHealth + pointsToAdd).Clamp(1, _engine.Settings.MaxHullHealth);
        }

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
        {
            ShieldHealth = (ShieldHealth + pointsToAdd).Clamp(1, _engine.Settings.MaxShieldHealth);
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
            _image = _engine.Assets.GetBitmap(imagePath);
            _size = new Size((int)_image.Size.Width, (int)_image.Size.Height);
        }

        /// <summary>
        /// Sets the size of the sprite. This is generally set by a call to SetImage() but some sprites (such as particles) have no images.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
        {
            _size = size;
        }

        #region Intersections.

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsAABB(SpriteBase otherObject)
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

                for (int i = 0; i < otherObject.Speed; i++)
                {
                    previousPosition.X -= otherObject.Orientation.X;
                    previousPosition.Y -= otherObject.Orientation.Y;

                    if (IntersectsAABB(previousPosition))
                    {
                        return true;

                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(SpriteBase otherObject, SiVector sizeAdjust)
        {
            if (Visable && otherObject.Visable && !IsQueuedForDeletion && !otherObject.IsQueuedForDeletion)
            {
                var alteredHitBox = new RectangleF(
                    otherObject.Bounds.X - (sizeAdjust.X / 2.0f),
                    otherObject.Bounds.Y - (sizeAdjust.Y / 2.0f),
                    otherObject.Bounds.Width + (sizeAdjust.X / 2.0f),
                    otherObject.Bounds.Height + (sizeAdjust.Y / 2.0f));

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
                (with.Bounds.X - variance),
                (with.Bounds.Y - variance),
                with.Size.Width + variance * 2, with.Size.Height + variance * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(SiVector location, SiVector size)
        {
            var alteredHitBox = new RectangleF(
                location.X,
                location.Y,
                size.X,
                size.Y
                );

            return Bounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool RenderLocationIntersectsAABB(SiVector location, SiVector size)
        {
            var alteredHitBox = new RectangleF(
                location.X,
                location.Y,
                size.X,
                size.Y
                );

            return RenderBounds.IntersectsWith(alteredHitBox);
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <returns></returns>
        public bool IntersectsAABB(SiVector location)
        {
            var alteredHitBox = new RectangleF(location.X, location.Y, 1f, 1f);
            return Bounds.IntersectsWith(alteredHitBox);
        }

        #endregion

        #region Actions.

        /// <summary>
        /// Subtract from the objects hullHealth.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(int damage)
        {
            if (ShieldHealth > 0)
            {
                _engine.Audio.PlayRandomShieldHit();
                damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                damage = damage < 1 ? 1 : damage;
                damage = damage > ShieldHealth ? ShieldHealth : damage; //No need to go negative with the damage.
                ShieldHealth -= damage;

                OnHit?.Invoke(this, SiDamageType.Shield, damage);
            }
            else
            {
                _engine.Audio.PlayRandomHullHit();
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
            Hit(munition?.Weapon.Metadata?.Damage ?? 0);
        }

        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the PointingAngle.
        /// </summary>
        public void RotatePointingDirection(float radians)
        {
            Orientation.Rotate(radians);
            RotationChanged();
        }

        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the PointingAngle.
        /// </summary>
        public void RotateMovementVector(float radians)
        {
            MovementVector.Rotate(radians);
            Orientation = MovementVector.Normalize();
            RotationChanged();
        }

        /// <summary>
        /// Instantly points a sprite at another by rotating the movement vector and then recalculates the PointingAngle.
        /// </summary>
        public void RotateMovementVector(SiVector toLocatipnOf)
        {
            var radians = Location.AngleToInSignedRadians(toLocatipnOf);

            MovementVector.SetDirctionMaintainMagnitude(radians);
            Orientation = MovementVector.Normalize();
        }

        /// <summary>
        /// Rotates the objects movment vector by the specified amount if it not pointing at the target
        ///     angle (with given tolerance) then recalculates PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SpriteBase obj, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInUnsignedDegrees(obj);

            if (deltaAngle.IsBetween(-varianceDegrees, varianceDegrees) == false)
            {
                if (deltaAngle >= -varianceDegrees)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (deltaAngle < varianceDegrees)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        /// angle (with given tolerance) then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SiVector toLocation, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInUnsignedDegrees(toLocation);

            if (deltaAngle.IsBetween(-varianceDegrees, varianceDegrees) == false)
            {
                if (deltaAngle >= -varianceDegrees)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (deltaAngle < varianceDegrees)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target angle
        /// (with given tolerance) then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(float toDegrees, float rotationAmountDegrees, float tolerance = 10)
        {
            toDegrees = toDegrees.DegreesNormalized();

            if (Orientation.DegreesSigned.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the given amount if it is pointing in the given direction then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if the object is not pointing in the given direction.
        public bool RotateMovementVectorIfPointingAt(SpriteBase obj, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInUnsignedDegrees(obj);

            if (deltaAngle.IsBetween(-varianceDegrees, varianceDegrees))
            {
                if (deltaAngle >= -varianceDegrees)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (deltaAngle < varianceDegrees)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }

                RecalculateMovementVector();

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

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            _engine.Sprites.Animations.AddRandomHitExplosionAt(this);
        }

        #endregion

        #region Sprite geometry.

        /// <summary>
        /// Returns true if any of the given sprites are pointing at this one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="atObjs"></param>
        /// <param name="toleranceDegrees"></param>
        /// <returns></returns>
        public bool IsPointingAtAny<T>(List<T> atObjs, float toleranceDegrees) where T : SpriteBase
        {
            foreach (var atObj in atObjs)
            {
                if (this.IsPointingAt(atObj, toleranceDegrees))
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
        public List<T> GetPointingAtOf<T>(List<T> atObjs, float toleranceDegrees) where T : SpriteBase
        {
            var results = new List<T>();

            foreach (var atObj in atObjs)
            {
                if (this.IsPointingAt(atObj, toleranceDegrees))
                {
                    results.Add(atObj);
                }
            }
            return results;
        }


        public float DistanceTo(SpriteBase to) => SiVector.DistanceTo(Location, to.Location);

        public float DistanceSquaredTo(SpriteBase to) => SiVector.DistanceSquaredTo(Location, to.Location);

        public float DistanceTo(SiVector to) => SiVector.DistanceTo(Location, to);

        /// <summary>
        /// Of the given sprites, returns the sprite that is the closest.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tos"></param>
        /// <returns></returns>
        public T ClosestOf<T>(List<T> tos) where T : SpriteBase
        {
            float closestDistance = float.MaxValue;
            T closestSprite = tos.First();

            foreach (var to in tos)
            {
                var distance = SiVector.DistanceTo(Location, to.Location);
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
        public float ClosestDistanceOf<T>(List<T> tos) where T : SpriteBase
        {
            float closestDistance = float.MaxValue;

            foreach (var to in tos)
            {
                var distance = SiVector.DistanceTo(Location, to.Location);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                };
            }

            return closestDistance;
        }

        #endregion

        /// <summary>
        /// Moves the sprite based on its movement vector and the epoch.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Perform any auto-rotation.
            Orientation.DegreesUnsigned += RotationSpeed * epoch;

            //Be default, we keep the MovementVector following the direction the sprite is pointing.
            //MovementVector = Direction * MovementVector.Length(); //This feel like a hack...

            //Move the sprite based on its vector.
            Location += MovementVector * epoch;

            foreach (var attachment in Attachments)
            {
                attachment.ApplyMotion(epoch, displacementVector);
            }
        }

        public virtual void VelocityChanged() { }
        public virtual void VisibilityChanged() { }
        public virtual void LocationChanged() { }
        public virtual void RotationChanged() { }

        public virtual void Cleanup()
        {
            Visable = false;

            _engine.Sprites.QueueAllForDeletionByOwner(UID);

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

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangleAt(renderTarget, RawRenderBounds, Orientation.RadiansSigned, _engine.Rendering.Materials.Colors.Red, 0, 1);
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
                if (this is SpriteEnemyBase)
                {
                    _engine.Rendering.HollowTriangleAt(renderTarget, x, y, 3, 3, _engine.Rendering.Materials.Colors.OrangeRed);
                }
                else if (this is MunitionBase)
                {
                    float size;
                    RawColor4 color;

                    var munition = this as MunitionBase;
                    if (munition.FiredFromType == SiFiredFromType.Enemy)
                    {
                        color = _engine.Rendering.Materials.Colors.Red;
                    }
                    else
                    {
                        color = _engine.Rendering.Materials.Colors.Green;
                    }

                    if (munition.Weapon.Metadata.ExplodesOnImpact)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    _engine.Rendering.FillEllipseAt(renderTarget, x, y, size, size, color);
                }
            }
        }

        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            _engine.Rendering.DrawBitmapAt(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }

        #endregion
    }
}
