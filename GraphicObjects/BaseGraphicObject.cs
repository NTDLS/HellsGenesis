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
        private readonly List<IWeapon> _weapons = new List<IWeapon>();

        private const string _assetExplosionAnimationPath = @"..\..\Assets\Graphics\Animation\Explode\";
        private readonly string[] _assetExplosionAnimationFiles = {
            #region Image Paths.
            "Explosion 256 1.png",
            "Explosion 256 2.png",
            "Explosion 256 3.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"..\..\Assets\Sounds\Explode\";
        private readonly string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "Expload 1.wav",
            "Expload 2.wav",
            "Expload 3.wav",
            "Expload 4.wav",
            "Expload 5.wav"
            #endregion
        };

        #region Properties.

        public RotationMode RotationMode { get; set; }
        public IWeapon CurrentWeapon { get; private set; }
        public int HitPoints { get; set; }
        public VelocityD Velocity { get; set; }

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

        private PointD _location = new PointD();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public PointD Location
        {
            get
            {
                return new PointD(_location);
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
                Invalidate();
            }
        }

        public PointD LocationCenter
        {
            get
            {
                return new PointD(_location.X - (Size.Width / 2.0), _location.Y - (Size.Height / 2.0));
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
                return new RectangleF(
                    (float)(_location.X - (Size.Width / 2.0)),
                    (float)(_location.Y - (Size.Height / 2.0)),
                    Size.Height, Size.Width);
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

                var invalidRect = new Rectangle(
                    (int)(_location.X - (_size.Width / 2.0)),
                    (int)(_location.Y - (_size.Height / 2.0)),
                    _size.Width, _size.Height);

                _core.Display.DrawingSurface.Invalidate(invalidRect);
            }
        }

        #endregion

        public BaseGraphicObject(Core core)
        {
            _core = core;
            RotationMode = RotationMode.Upsize;
        }

        public void LoadResources(string imagePath, Size? size = null, string hitSoundPath = null,
            string explodeSoundPath = null, PointD initialLocation = null, VelocityD initialVector = null)
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
                _location.X = Utility.Random.Next(0, _core.Display.VisibleSize.Width - _size.Width);
                _location.Y = Utility.Random.Next(0, _core.Display.VisibleSize.Height - _size.Height);
            }
            else
            {
                _location.X = (int)initialLocation?.X;
                _location.Y = (int)initialLocation?.Y;
            }

            ReadyForDeletion = false;

            if (initialVector == null)
            {
                Velocity = new VelocityD();
                Velocity.MaxSpeed = Utility.Random.Next(Consants.Limits.MinSpeed, Consants.Limits.MaxSpeed);
                Velocity.Angle.Degrees = Utility.Random.Next(0, 360);
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

        public void AddWeapon(IWeapon weapon)
        {
            weapon.SetOwner(this);
            _weapons.Add(weapon);
        }

        public IWeapon SelectNextAvailableUsableWeapon()
        {
            bool selectNextWeapon = false;

            foreach (var weapon in _weapons)
            {
                if (selectNextWeapon)
                {
                    CurrentWeapon = weapon;
                    return weapon;
                }

                if (weapon == CurrentWeapon) //Find the current weapon in the collection;
                {
                    selectNextWeapon = true;
                }
            }

            return SelectFirstAvailableUsableWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public IWeapon SelectFirstAvailableUsableWeapon()
        {
            var existingWeapon = (from o in _weapons where o.RoundQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                CurrentWeapon = existingWeapon;
            }
            else
            {
                CurrentWeapon = null;
            }
            return CurrentWeapon;
        }

        public IWeapon SelectWeapon(Type weaponType)
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
            var invalidRect = new Rectangle(
                (int)(_location.X - (_size.Width / 2.0)),
                (int)(_location.Y - (_size.Height / 2.0)),
                _size.Width, _size.Height);
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
            Velocity.Angle.Degrees += degrees;
            Invalidate();
        }

        public void MoveInDirectionOf(PointD location, double? speed = null)
        {
            this.Velocity.Angle.Degrees = PointD.AngleTo(this.Location, location);
            if (speed != null)
            {
                this.Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(BaseGraphicObject obj, double? speed = null)
        {
            this.Velocity.Angle.Degrees = PointD.AngleTo(this.Location, obj.Location);

            if (speed != null)
            {
                this.Velocity.MaxSpeed = (double)speed;
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
                if (Velocity.Angle.Degrees != 0 && RotationMode != RotationMode.None)
                {
                    if (RotationMode == RotationMode.Upsize) //Very expensize
                    {
                        var bitmap = new Bitmap(_image);
                        var image = Utility.RotateImageWithUpsize(bitmap, Velocity.Angle.Degrees, Color.Transparent);
                        Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                        dc.DrawImage(image, rect);

                        _size.Height = image.Height;
                        _size.Width = image.Width;
                    }
                    else if (RotationMode == RotationMode.Clip) //Much less expensive.
                    {
                            var bitmap = new Bitmap(_image);
                        var image = Utility.RotateImageWithClipping(bitmap, Velocity.Angle.Degrees, Color.Transparent);
                        Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                        dc.DrawImage(image, rect);

                        _size.Height = image.Height;
                        _size.Width = image.Width;
                    }
                }
                else //Almost free.
                {
                    Rectangle rect = new Rectangle((int)(_location.X - (_image.Width / 2.0)), (int)(_location.Y - (_image.Height / 2.0)), _image.Width, _image.Height);
                    dc.DrawImage(_image, rect);
                    dc.DrawImage(_image, rect);
                }
            }
        }

        #endregion
    }
}
