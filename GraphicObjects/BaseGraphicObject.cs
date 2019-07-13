using AI2D.Engine;
using AI2D.GraphicObjects.Bullets;
using AI2D.GraphicObjects.Enemies;
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
        private Image _lockedOnImage;
        private Image _lockedOnSoftImage;
        private ObjAnimation _explosionAnimation;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private int _MilisecondsBetweenHits = 100;
        private AudioClip _hitSound;
        private readonly List<WeaponBase> _weapons = new List<WeaponBase>();

        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.

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
        public WeaponBase CurrentWeapon { get; private set; }
        public int HitPoints { get; set; }
        public VelocityD Velocity { get; set; } = new VelocityD();

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
                    _core.Actors.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

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

        public bool IsOnScreen
        {
            get
            {
                return _core.Display.VisibleBounds.IntersectsWith(Bounds);
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
        public virtual Size Size
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
                return _isVisible && !_readyForDeletion;
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

        public void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Actors.GetSoundCached(@"..\..\Assets\Sounds\Object Hit.wav", 0.65f);

            int _explosionSoundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Actors.GetSoundCached(_assetExplosionSoundPath + _assetExplosionSoundFiles[_explosionSoundIndex], 1.0f);

            int _explosionImageIndex = Utility.RandomNumber(0, _assetExplosionAnimationFiles.Count());
            _explosionAnimation = new ObjAnimation(_core, _assetExplosionAnimationPath + _assetExplosionAnimationFiles[_explosionImageIndex], new Size(256, 256));

            _lockedOnImage = _core.Actors.GetBitmapCached(@"..\..\Assets\Graphics\Weapon\Locked On.png");
            _lockedOnSoftImage = _core.Actors.GetBitmapCached(@"..\..\Assets\Graphics\Weapon\Locked Soft.png");

            if (imagePath != null)
            {
                SetImage(imagePath, size);
            }
        }

        public void ClearWeapons()
        {
            _weapons.Clear();
        }

        public void AddWeapon(WeaponBase weapon)
        {
            var existing = GetWeaponOfType(weapon.GetType());
            if (existing == null)
            {
                weapon.SetOwner(this);
                _weapons.Add(weapon);
            }
            else
            {
                existing.RoundQuantity += weapon.RoundQuantity;
            }
        }

        public int TotalAvailableRounds()
        {
            return (from o in _weapons select o.RoundQuantity).Sum();
        }

        public int TotalFiresRounds()
        {
            return (from o in _weapons select o.RoundsFired).Sum();
        }

        public WeaponBase SelectNextAvailableUsableWeapon()
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

        public bool HasWeapon(Type weaponType)
        {
            var existingWeapon = (from o in _weapons where o.GetType() == weaponType select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasWeaponAndAmmo(Type weaponType)
        {
            var existingWeapon = (from o in _weapons where o.GetType() == weaponType select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public WeaponBase SelectFirstAvailableUsableWeapon()
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

        public WeaponBase GetWeaponOfType(Type weaponType)
        {
            return (from o in _weapons where o.GetType() == weaponType select o).FirstOrDefault();
        }

        public WeaponBase SelectWeapon(Type weaponType)
        {
            CurrentWeapon = GetWeaponOfType(weaponType);
            return CurrentWeapon;
        }

        public void SetImage(Image image, Size? size = null)
        {
            _image = image;

            if (size != null)
            {
                _image = Utility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public void SetImage(string imagePath, Size? size = null)
        {
            _image = _core.Actors.GetBitmapCached(imagePath);

            if (size != null)
            {
                _image = Utility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }

            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public Image GetImage()
        {
            return _image;
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

        /// <summary>
        /// Allows for intersect detection with larger/smaller "hit box".
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="sizeAdjust"></param>
        /// <returns></returns>
        public bool Intersects(BaseGraphicObject otherObject, PointD sizeAdjust)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                var alteredHitBox = new RectangleF(
                    otherObject.Bounds.X - (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Y - (float)(sizeAdjust.Y / 2),
                    otherObject.Bounds.Width + (float)(sizeAdjust.X / 2),
                    otherObject.Bounds.Height + (float)(sizeAdjust.Y / 2));

                return this.Bounds.IntersectsWith(alteredHitBox);
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

        public bool Hit(BaseBullet bullet)
        {
            if (bullet != null)
            {
                return Hit(bullet.Weapon.Damage);
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

        /// <summary>
        /// Calculated the difference in heading angle from one object to get to another.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double DeltaAngle(BaseGraphicObject atObj)
        {
            return Utility.DeltaAngle(this, atObj);
        }

        /// <summary>
        /// Calculates the angle in degrees of one objects location to another location.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double AngleTo(BaseGraphicObject atObj)
        {
            return Utility.AngleTo(this, atObj);
        }

        public bool IsPointingAt(BaseGraphicObject atObj, double toleranceDegrees)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(BaseGraphicObject to)
        {
            return PointD.DistanceTo(this.Location, to.Location);
        }
        public double DistanceTo(PointD to)
        {
            return PointD.DistanceTo(this.Location, to);
        }

        public void Explode()
        {
            if (this is BaseEnemy)
            {
                _core.Actors.Player.Score += (this as BaseEnemy).ScorePoints;
            }

            _explodeSound.Play();
            _explosionAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_explosionAnimation, this);
            ReadyForDeletion = true;
        }

        public virtual void Cleanup()
        {
            Visable = false;
            this.Invalidate(); //Don't think this is necessary. Just seems right.
        }

        public void Render(Graphics dc)
        {
            if (_isVisible && _image != null)
            {
                DrawImage(dc, _image);

                if (_lockedOnImage != null && IsLockedOn)
                {
                    DrawImage(dc, _lockedOnImage, 0);
                }
                else if (_lockedOnImage != null && IsLockedOnSoft)
                {
                    DrawImage(dc, _lockedOnSoftImage, 0);
                }
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double) (angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            Bitmap bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = Utility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = Utility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    Rectangle rect = new Rectangle((int)(_location.X - (image.Width / 2.0)), (int)(_location.Y - (image.Height / 2.0)), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(_location.X - (bitmap.Width / 2.0)), (int)(_location.Y - (bitmap.Height / 2.0)), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        #endregion
    }
}
