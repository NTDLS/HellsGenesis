using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Sprite;
using System.Collections.Generic;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._SpriteBase
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        #region Backend variables.

        protected EngineCore _engine;

        private SharpDX.Direct2D1.Bitmap _image;
        private bool _readyForDeletion;
        private SiVector _location = new();
        private Size _size;

        #endregion

        #region Travel Vector.

        private float _speed;

        /// <summary>
        /// The speed that this object can generally travel in any direction.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                //RecalculateMovementVector(); //Seems like unneeded overhead.
            }
        }

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
        /// Percentage of speed expressed as a decimal percentage from 0.0 (stopped) to float.max.
        /// Note that a throttle of 2.0 is twize the normal speed.
        /// </summary>
        public float Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value.Clamp(0, float.MaxValue);
                //RecalculateMovementVector(); //Seems like unneeded overhead.
            }
        }


        private float _maxThrottle = 1.0f;
        /// <summary>
        /// The general maximum throttle that can be applied. This can be considered the "boost" speed.
        /// </summary>
        public float MaxThrottle
        {
            get => _maxThrottle;
            set => _maxThrottle = value.Clamp(0, float.MaxValue);
        }

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

        /// <summary>
        /// The bounds of the sprite in the universe.
        /// </summary>
        public virtual RectangleF Bounds => new(
                Location.X - Size.Width / 2.0f,
                Location.Y - Size.Height / 2.0f,
                Size.Width,
                Size.Height);

        /// <summary>
        /// The raw bounds of the sprite in the universe.
        /// </summary>
        public virtual RawRectangleF RawBounds => new(
                        Location.X - Size.Width / 2.0f,
                        Location.Y - Size.Height / 2.0f,
                        Location.X - Size.Width / 2.0f + Size.Width,
                        Location.Y - Size.Height / 2.0f + Size.Height);

        /// <summary>
        /// The bounds of the sprite on the display.
        /// </summary>
        public virtual RectangleF RenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        Size.Width,
                        Size.Height);

        /// <summary>
        /// The raw bounds of the sprite on the display.
        /// </summary>
        public virtual RawRectangleF RawRenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        RenderLocation.X - Size.Width / 2.0f + Size.Width,
                        RenderLocation.Y - Size.Height / 2.0f + Size.Height);


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
            get => _location - Size / 2.0f; //Changes made to the location object do not affect the sprite.
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

        // The Z location. Given that this is a 2d engine, the Z order is just a render order.
        public int Z { get; set; } = 0;

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


        public SpriteBase(EngineCore engine, string spriteTag = "")
        {
            _engine = engine;

            SpriteTag = spriteTag;
            IsHighlighted = _engine.Settings.HighlightAllSprites;
        }

        /// <summary>
        /// Sets the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector() => MovementVector = MakeMovementVector();

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(float angleInRadians) => MovementVector = MakeMovementVector(angleInRadians);

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(SiVector angle) => MovementVector = MakeMovementVector(angle);

        /// <summary>
        /// Returns the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector() => Orientation * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(float angleInRadians) => new SiVector(angleInRadians) * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(SiVector angle) => angle.Normalize() * Speed * Throttle;

        public void QueueForDelete()
        {
            _readyForDeletion = true;
            Visable = false;

            foreach (var attachment in Attachments)
            {
                attachment.QueueForDelete();
            }

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

        /// <summary>
        /// Moves the sprite based on its movement vector and the epoch.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Perform any auto-rotation.
            Orientation.Radians += RotationSpeed * epoch;

            //Move the sprite based on its vector.
            Location += MovementVector * epoch;
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
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
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
                    _engine.Rendering.DrawTriangle(renderTarget, x, y, 3, 3, _engine.Rendering.Materials.Colors.OrangeRed);
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

                    _engine.Rendering.DrawSolidEllipse(renderTarget, x, y, size, size, color);
                }
            }
        }

        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            _engine.Rendering.DrawBitmap(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }

        #endregion
    }
}
