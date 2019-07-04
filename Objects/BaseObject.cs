using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;
using System.Linq;

namespace AI2D.Objects
{
    public class BaseObject
    {
        private Game _game;
        private Image _image;
        private Animation _explosionAnimation;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenHits = 100;
        private DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenBullets = 100;
        private AudioClip _bulletSound;
        private AudioClip _hitSound;

        private string _assetPathFramePath = @"..\..\Assets\Graphics\Frames\";
        private string[] _explosionImageNames = {
            #region images.
            "Explosion 256 1.png",
            "Explosion 256 2.png",
            "Explosion 256 3.png",
            "Explosion 256 4.png",
            #endregion
        };

        public BaseObject(Game game)
        {
            _game = game;
        }

        public void LoadResources(string imagePath, Size? size = null, PointD initialLocation = null, Vector initialVector = null)
        {
            _bulletSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\VulcanCannon.wav", 0.3f);
            _hitSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\ShipHit.wav", 1.0f);
            _explodeSound = _game.Actors.GetAudioClip(@"..\..\Assets\Sound\Boom.wav", 1.0f);

            int _explosionImageIndex = Utility.Random.Next(0, 1000) % _explosionImageNames.Count();
            _explosionAnimation = new Animation(_game, _assetPathFramePath + _explosionImageNames[_explosionImageIndex], new Size(256, 256));

            if (imagePath != null)
            {
                _image = _game.Actors.GetBitmap(imagePath);
                if (size == null)
                {
                    _size = new Size(_image.Size.Width, _image.Size.Height);
                }
            }

            if (size != null)
            {
                _image = Utility.ResizeImage(_image, size.Value.Width, size.Value.Height);
                _size = (Size)size;
            }

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

        public void SetImage(Image image)
        {
            _image = image;
            _size.Height = image.Height;
            _size.Width = image.Width;
            Invalidate();
        }

        #region Properties.

        public int HitPoints { get; set; }
        public Vector Velocity { get; set; }
        public double RotationSpeed { get; set; } = 1.0;

        private bool _readyForDeletion;
        public bool ReadyForDeletion
        {
            get
            {
                return _readyForDeletion;
            }
            set
            {
                _readyForDeletion = value;
                if (_readyForDeletion)
                {
                    Visable = false;
                }
            }
        }

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

        public void Invalidate()
        {
            var invalidRect = new Rectangle((int)_x, (int)_y, _size.Width, _size.Height);
            _game.Display.DrawingSurface.Invalidate(invalidRect);
        }

        public bool Intersects(BaseObject otherObject)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
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
            _explodeSound.Play();
            _explosionAnimation.Reset();
            _game.Actors.PlaceAnimationOnTopOf(_explosionAnimation, this);
            ReadyForDeletion = true;
        }

        public void Cleanup()
        {
            Visable = false;
        }

        public void Render(Graphics dc)
        {
            if (_isVisible && _image != null)
            {
                var bitmap = new Bitmap(_image);

                var image = Visuals.RotateImage(bitmap, Velocity.Angle.Degree);
                Rectangle rect = new Rectangle((int)_x, (int)_y, image.Width, image.Height);
                dc.DrawImage(image, rect);
            }
        }

        #endregion
    }
}
