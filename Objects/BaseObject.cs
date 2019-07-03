using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.Objects
{
    public class BaseObject
    {
        private Game _game;
        private Image _defaultImage;
        private AnimationFrames _explodeFrames;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenHits = 100;
        private DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenBullets = 100;
        private AudioClip _bulletSound;
        private AudioClip _hitSound;

        public void Initialize(Game game, string imagePath, Size? size = null, PointD initialLocation = null, Vector initialVector = null)
        {
            _game = game;

            _bulletSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\VulcanCannon.wav", 0.3f);
            _hitSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\ShipHit.wav", 1.0f);
            _explodeSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\Boom.wav", 1.0f);

            _explodeFrames = new AnimationFrames(_game, @"..\..\Assets\Graphics\Frames\Explosion 2.png", new Size(256, 256));

            _defaultImage = _game.Actors.GetBitmap(imagePath);
            _size = new Size(_defaultImage.Size.Width, _defaultImage.Size.Height);

            if (initialLocation == null)
            {
                _x = Utility.Random.Next(0, _game.Display.VisibleSize.Width - _size.Width);
                _y = Utility.Random.Next(0, _game.Display.VisibleSize.Height - _size.Height);
            }
            else
            {
                _x = (int)initialLocation?.X;
                _y = (int)initialLocation?.Y;
            }

            IsDead = false;
            ReadyForDeletion = false;

            if (initialVector == null)
            {
                Velocity = new Vector();
                Velocity.Speed = Utility.Random.Next(Consants.Limits.MinSpeed, Consants.Limits.MaxSpeed);
                Velocity.Angle.Degree = Utility.Random.Next(0, 360);
            }
            else
            {
                Velocity = initialVector;
            }
        }

        #region Properties.

        public int HitPoints { get; set; }
        public bool ReadyForDeletion { get; set; }
        public bool IsDead { get; set; }
        public Vector Velocity { get; set; }
        public double RotationSpeed { get; set; } = 1.0;

        public bool CanFire
        {
            get
            {
                bool result = ((DateTime.Now - _lastFired).TotalMilliseconds > _MilisecondsBetweenBullets);
                if (result)
                {
                    _lastFired = DateTime.Now;
                }
                return result;
            }
        }

        double _x = 0;
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                Invalidate();
                _x = value;
                Invalidate();
            }
        }

        double _y = 0;
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                Invalidate();
                _y = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public PointD Location
        {
            get
            {
                return new PointD(_x, _y);
            }
        }

        public PointF LocationF
        {
            get
            {
                return new PointF((float)_x, (float)_y);
            }
        }

        private Size _size;
        public Size Size
        {
            get
            {
                return _size;
            }
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF((int)_x, (int)_y, Size.Height, Size.Width);
            }
        }

        private bool _isVisible = true;
        public bool Visable
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                var invalidRect = new Rectangle((int)_x, (int)_y, _size.Width, _size.Height);
                _game.Display.DrawingSurface.Invalidate(invalidRect);
            }
        }

        #endregion

        private Bitmap GetCurrentImage()
        {
            if (this.IsDead)
            {
                Bitmap replacementImage = _explodeFrames.GetReplacmentImage();
                if (replacementImage == null)
                {
                    this.ReadyForDeletion = true;
                }
                return replacementImage;
            }

            return Visuals.RotateImage(new Bitmap(_defaultImage), Velocity.Angle.Degree);
        }

        public void Invalidate()
        {
            var invalidRect = new Rectangle((int)_x, (int)_y, _size.Width, _size.Height);
            _game.Display.DrawingSurface.Invalidate(invalidRect);
        }

        /// <summary>
        /// Gives the object the oppratunity to change do anything before the frame advances.
        /// </summary>
        public void AdvanceFrame()
        {
            if (this.ReadyForDeletion)
            {
                return;
            }

            //If we are dead then we are going to show the explode animation. We'll want to make sure every frame is invalidated on the dc.
            if (this.IsDead)
            {
                Invalidate();
            }
        }

        public bool Intersects(BaseObject otherObject)
        {
            if (IsDead == false && otherObject.IsDead == false && ReadyForDeletion == false && otherObject.ReadyForDeletion == false)
            {
                return this.Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        #region Actions.

        /// <summary>
        /// Subtract from the objects hitpoints.
        /// </summary>
        /// <returns></returns>
        public bool Hit()
        {
            bool result = ((DateTime.Now - _lastHit).TotalMilliseconds > _MilisecondsBetweenHits);
            if (result)
            {
                _hitSound.Play();
                _lastHit = DateTime.Now;
                HitPoints--;

                if (HitPoints <= 0)
                {
                    Explode();
                }
            }
            return result;
        }

        public void FireGun()
        {
            if (CanFire)
            {
                _bulletSound.Play();
                _game.Actors.CreateBullet(this);
            }
        }

        public void Rotate(double degrees)
        {
            Velocity.Angle.Degree += degrees;
            Invalidate();
        }

        public void MoveInDirectionOf(PointD location, double? speed = null)
        {
            this.Velocity.Angle.Degree = Utility.CalculeAngle(this.Location, location);
            if (speed != null)
            {
                this.Velocity.Speed = (double)speed;
            }
        }

        public void MoveInDirectionOf(BaseObject obj, double? speed = null)
        {
            this.Velocity.Angle.Degree = Utility.CalculeAngle(this.Location, obj.Location);

            if (speed != null)
            {
                this.Velocity.Speed = (double)speed;
            }
        }

        public void Explode()
        {
            IsDead = true;
            _explodeSound.Play();
            //explodeFrameController.Play();
        }

        public void Cleanup()
        {
            Visable = false;
        }

        public void Render(Graphics dc)
        {
            if (_isVisible)
            {
                var image = GetCurrentImage();
                if (image != null)
                {
                    Rectangle rect = new Rectangle((int)_x, (int)_y, Size.Width, Size.Height);
                    dc.DrawImage(image, rect);
                }
            }
        }

        #endregion
    }
}
