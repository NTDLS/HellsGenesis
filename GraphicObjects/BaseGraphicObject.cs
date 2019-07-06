using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects
{
    public class BaseGraphicObject
    {
        protected Core _core;

        private Image _image;
        private ObjAnimation _explosionAnimation;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenHits = 100;
        private AudioClip _hitSound;
        private List<WeaponBase> _weapons = new List<WeaponBase>();

        private string _assetExplosionAnimationPath = @"..\..\Assets\Graphics\Animation\Explode\";
        private string[] _assetExplosionAnimationFiles = {
            #region Image Paths.
            "Explosion 256 1.png",
            "Explosion 256 2.png",
            "Explosion 256 3.png",
            "Explosion 256 4.png",
            #endregion
        };

        private string _assetExplosionSoundPath = @"..\..\Assets\Sounds\Explode\";
        private string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "Expload 1.wav",
            "Expload 2.wav",
            "Expload 3.wav",
            "Expload 4.wav",
            "Expload 5.wav"
            #endregion
        };

        #region Properties.

        public WeaponBase CurrentWeapon { get; private set; }

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
                return new RectangleF((float)_x, (float)_y, Size.Height, Size.Width);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)_x, (int)_y, Size.Height, Size.Width);
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
                _core.Display.DrawingSurface.Invalidate(invalidRect);
            }
        }

        #endregion

        public BaseGraphicObject(Core core)
        {
            _core = core;
        }


        public void LoadResources(string imagePath, Size? size = null, string hitSoundPath = null,
            string explodeSoundPath = null, PointD initialLocation = null, Vector initialVector = null)
        {
            if (hitSoundPath == null)
            {
                _hitSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Ship Hit.wav", 0.65f);
            }
            else
            {
                _hitSound = _core.Actors.GetSoundCached(hitSoundPath, 0.65f);
            }

            if (explodeSoundPath == null)
            {
                int _explosionSoundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
                _explodeSound = _core.Actors.GetSoundCached(_assetExplosionSoundPath + _assetExplosionSoundFiles[_explosionSoundIndex], 1.0f);
            }
            else
            {
                _explodeSound = _core.Actors.GetSoundCached(explodeSoundPath, 1.0f);
            }

            int _explosionImageIndex = Utility.RandomNumber(0, _assetExplosionAnimationFiles.Count());
            _explosionAnimation = new ObjAnimation(_core, _assetExplosionAnimationPath + _assetExplosionAnimationFiles[_explosionImageIndex], new Size(256, 256));

            if (imagePath != null)
            {
                _image = _core.Actors.GetBitmapCached(imagePath);
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
                _x = Utility.Random.Next(0, _core.Display.VisibleSize.Width - _size.Width);
                _y = Utility.Random.Next(0, _core.Display.VisibleSize.Height - _size.Height);
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

        public void ClearWeapons()
        {
            _weapons.Clear();
        }

        public void AddWeapon(WeaponBase weapon)
        {
            weapon.SetOwner(this);
            _weapons.Add(weapon);
        }

        public WeaponBase SelectWeapon(Type weaponType)
        {
            var existingWeapon = (from o in _weapons where o.GetType() == weaponType select o).FirstOrDefault();

            CurrentWeapon = existingWeapon;

            return existingWeapon;
        }

        public void SetImage(Image image)
        {
            _image = image;
            _size.Height = image.Height;
            _size.Width = image.Width;
            Invalidate();
        }

        public void Invalidate()
        {
            var invalidRect = new Rectangle((int)_x, (int)_y, _size.Width, _size.Height);
            _core.Display.DrawingSurface.Invalidate(invalidRect);
        }

        public bool Intersects(BaseGraphicObject otherObject)
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
        public bool Hit(int damage)
        {
            bool result = ((DateTime.Now - _lastHit).TotalMilliseconds > _MilisecondsBetweenHits);
            if (result)
            {
                _hitSound.Play();
                _lastHit = DateTime.Now;

                HitPoints -= damage > HitPoints ? HitPoints : damage; //No need to go negative with the damage.

                if (HitPoints <= 0)
                {
                    Explode();
                }
            }
            return result;
        }

        public bool Hit(ObjBullet bullet)
        {
            if (bullet != null)
            {
                return Hit(bullet.Damage);
            }
            return false;
        }

        public void Rotate(double degrees)
        {
            Velocity.Angle.Degree += degrees;
            Invalidate();
        }

        public void MoveInDirectionOf(PointD location, double? speed = null)
        {
            this.Velocity.Angle.Degree = Utility.RequiredAngleTo(this.Location, location);
            if (speed != null)
            {
                this.Velocity.Speed = (double)speed;
            }
        }

        public void MoveInDirectionOf(BaseGraphicObject obj, double? speed = null)
        {
            this.Velocity.Angle.Degree = Utility.RequiredAngleTo(this.Location, obj.Location);

            if (speed != null)
            {
                this.Velocity.Speed = (double)speed;
            }
        }

        public double GetDeltaAngle(BaseGraphicObject atObj)
        {
            return Utility.GetDeltaAngle(this, atObj);
        }

        public double RequiredAngleTo(BaseGraphicObject atObj)
        {
            return Utility.RequiredAngleTo(this, atObj);
        }

        public bool IsPointingAt(BaseGraphicObject atObj, double toleranceDegrees)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public void Explode()
        {
            _explodeSound.Play();
            _explosionAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_explosionAnimation, this);
            ReadyForDeletion = true;
        }

        public void Cleanup()
        {
            Visable = false;
            this.Invalidate(); //Don't think this is necessary. Just seems right.
        }

        public void Render(Graphics dc)
        {
            if (_isVisible && _image != null)
            {
                var bitmap = new Bitmap(_image);

                var image = Utility.RotateImage(bitmap, Velocity.Angle.Degree);
                Rectangle rect = new Rectangle((int)_x, (int)_y, _image.Width, _image.Height);
                dc.DrawImage(image, rect);
            }
        }

        #endregion
    }
}
