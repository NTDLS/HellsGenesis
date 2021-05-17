using AI2D.Engine;
using AI2D.GraphicObjects.Bullets;
using AI2D.GraphicObjects.Enemies;
using AI2D.GraphicObjects.PowerUp;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects
{
    public class ActorBase
    {
        public delegate void PositionChanged(ActorBase obj);
        public event PositionChanged OnPositionChanged;

        public delegate void Rotated(ActorBase obj);
        public event Rotated OnRotated;

        public delegate void VisibilityChange(ActorBase obj);
        public event VisibilityChange OnVisibilityChange;


        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;

        private Image _image;
        private Image _lockedOnImage;
        private Image _lockedOnSoftImage;
        private ObjAnimation _explosionAnimation;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private int _MillisecondsBetweenHits = 200;
        private AudioClip _hitSound;
        private AudioClip _shieldHit;
        private readonly List<WeaponBase> _weapons = new List<WeaponBase>();
        private ObjAnimation _hitExplosionAnimation { get; set; }

        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.

        private const string _assetExplosionAnimationPath = @"..\..\..\Assets\Graphics\Animation\Explode\";
        private readonly string[] _assetExplosionAnimationFiles = {
            #region Image Paths.
            "Explosion 256 1.png",
            "Explosion 256 2.png",
            "Explosion 256 3.png",
            #endregion
        };

        private const string _assetExplosionSoundPath = @"..\..\..\Assets\Sounds\Explode\";
        private readonly string[] _assetExplosionSoundFiles = {
            #region Sound Paths.
            "Expload 1.wav",
            "Expload 2.wav",
            "Expload 3.wav",
            "Expload 4.wav",
            "Expload 5.wav"
            #endregion
        };

        private const string _assetHitExplosionAnimationPath = @"..\..\..\Assets\Graphics\Animation\Explode\";
        private readonly string[] _assetHitExplosionAnimationFiles = {
            #region Image Paths.
            "Hit Explosion 22 (1).png",
            "Hit Explosion 22 (2).png"
            #endregion
        };

        #region Properties.

        public RotationMode RotationMode { get; set; }
        public WeaponBase CurrentWeapon { get; private set; }
        public int HitPoints { get; private set; } = 0;
        public int ShieldPoints { get; private set; } = 0;
        public VelocityD Velocity { get; set; } = new VelocityD();
        public bool IsDead { get; set; } = false;

        public void SetHitPoints(int points)
        {
            HitPoints = 0;
            AddHitPoints(points);
        }

        public void AddHitPoints(int pointsToAdd)
        {
            if (HitPoints + pointsToAdd > Constants.Limits.MaxHitpoints)
            {
                pointsToAdd = Constants.Limits.MaxHitpoints - (HitPoints + pointsToAdd);
            }

            /*
            if (this is ObjPlayer)
            {
                var player = this as ObjPlayer;

                if (HitPoints < Constants.Limits.MaxShieldPoints && HitPoints + pointsToAdd >= Constants.Limits.MaxShieldPoints)
                {
                    player.AllSystemsGoSound.Play();
                }
            }
            */

            HitPoints += pointsToAdd;
        }

        public void SetShieldPoints(int points)
        {
            ShieldPoints = 0;
            AddShieldPoints(points);
        }

        public void AddShieldPoints(int pointsToAdd)
        {
            if (ShieldPoints + pointsToAdd > Constants.Limits.MaxShieldPoints)
            {
                pointsToAdd = Constants.Limits.MaxShieldPoints - (ShieldPoints + pointsToAdd);
            }

            if (this is ObjPlayer)
            {
                var player = this as ObjPlayer;

                if (ShieldPoints < Constants.Limits.MaxShieldPoints && ShieldPoints + pointsToAdd >= Constants.Limits.MaxShieldPoints)
                {
                    player.ShieldMaxSound.Play();
                }
            }

            ShieldPoints += pointsToAdd;
        }

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

        public void QueueForDelete()
        {
            Visable = false;
            if (_readyForDeletion == false)
            {
                OnVisibilityChange?.Invoke(this);
            }
            _readyForDeletion = true;
        }

        private bool _readyForDeletion;
        public bool ReadyForDeletion
        {
            get
            {
                return _readyForDeletion;
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
                OnPositionChanged?.Invoke(this);
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
                OnPositionChanged?.Invoke(this);
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
                return new RectangleF((float)(_location.X), (float)(_location.Y), Size.Width, Size.Height);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)(_location.X), (int)(_location.Y), Size.Width, Size.Height);
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

                    var invalidRect = new Rectangle(
                        (int)(_location.X - (_size.Width / 2.0)),
                        (int)(_location.Y - (_size.Height / 2.0)),
                        _size.Width, _size.Height);

                    _core.Display.DrawingSurface.Invalidate(invalidRect);

                    OnVisibilityChange?.Invoke(this);
                }
            }
        }

        #endregion

        public ActorBase(Core core)
        {
            _core = core;
            RotationMode = RotationMode.Upsize;
            Velocity.MaxRotationSpeed = Constants.Limits.MaxRotationSpeed;
        }

        public void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Object Hit.wav", 0.5f);
            _shieldHit = _core.Actors.GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Shield Hit.wav", 0.5f);

            int _explosionSoundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
            _explodeSound = _core.Actors.GetSoundCached(_assetExplosionSoundPath + _assetExplosionSoundFiles[_explosionSoundIndex], 1.0f);

            int _explosionImageIndex = Utility.RandomNumber(0, _assetExplosionAnimationFiles.Count());
            _explosionAnimation = new ObjAnimation(_core, _assetExplosionAnimationPath + _assetExplosionAnimationFiles[_explosionImageIndex], new Size(256, 256));

            _lockedOnImage = _core.Actors.GetBitmapCached(@"..\..\..\Assets\Graphics\Weapon\Locked On.png");
            _lockedOnSoftImage = _core.Actors.GetBitmapCached(@"..\..\..\Assets\Graphics\Weapon\Locked Soft.png");

            int _hitExplosionImageIndex = Utility.RandomNumber(0, _assetHitExplosionAnimationFiles.Count());
            _hitExplosionAnimation = new ObjAnimation(_core, _assetHitExplosionAnimationPath + _assetHitExplosionAnimationFiles[_hitExplosionImageIndex], new Size(22, 22));

            if (imagePath != null)
            {
                SetImage(imagePath, size);
            }

            OnVisibilityChange?.Invoke(this);
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

        public bool Intersects(ActorBase otherObject)
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
        public bool Intersects(ActorBase otherObject, PointD sizeAdjust)
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
            bool result = ((DateTime.Now - _lastHit).TotalMilliseconds > _MillisecondsBetweenHits);
            if (result)
            {
                _lastHit = DateTime.Now;

                if (ShieldPoints > 0)
                {
                    _shieldHit.Play();
                    damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                    damage = damage < 1 ? 1 : damage;
                    ShieldPoints -= damage > ShieldPoints ? ShieldPoints : damage; //No need to go negative with the damage.

                    if (this is ObjPlayer)
                    {
                        var player = this as ObjPlayer;
                        if (ShieldPoints == 0)
                        {
                            player.ShieldDownSound.Play();
                        }
                    }
                }
                else
                {
                    _hitSound.Play();
                    HitPoints -= damage > HitPoints ? HitPoints : damage; //No need to go negative with the damage.

                    if (this is ObjPlayer)
                    {
                        var player = this as ObjPlayer;
                        //This is the hit that took us under the treshold.
                        if (HitPoints < 100 && HitPoints + damage > 100)
                        {
                            player.IntegrityLowSound.Play();
                        }
                        else if (HitPoints < 50 && HitPoints + damage > 50)
                        {
                            player.SystemsFailingSound.Play();
                        }
                        else if (HitPoints < 20 && HitPoints + damage > 20)
                        {
                            player.HullBreachedSound.Play();
                        }                     
                    }

                    if (HitPoints <= 0)
                    {
                        Explode();
                    }
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
            OnRotated?.Invoke(this);
        }

        public void MoveInDirectionOf(PointD location, double? speed = null)
        {
            this.Velocity.Angle.Degrees = PointD.AngleTo(this.Location, location);
            if (speed != null)
            {
                this.Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(ActorBase obj, double? speed = null)
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
        public double DeltaAngle(ActorBase atObj)
        {
            return Utility.DeltaAngle(this, atObj);
        }

        /// <summary>
        /// Calculates the angle in degrees of one objects location to another location.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double AngleTo(ActorBase atObj)
        {
            return Utility.AngleTo(this, atObj);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees)
        {
            return Utility.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(ActorBase to)
        {
            return PointD.DistanceTo(this.Location, to.Location);
        }

        public double DistanceTo(PointD to)
        {
            return PointD.DistanceTo(this.Location, to);
        }

        public void Explode(bool autoKill = true, bool autoDelete = true)
        {
            if (this is BaseEnemy)
            {
                _core.Actors.Player.Score += (this as BaseEnemy).ScorePoints;

                //If the type of explosion is an enemy then maybe spawn a powerup.
                if (Utility.ChanceIn(5))
                {
                    BasePowerUp powerUp = Utility.FlipCoin() ? (BasePowerUp)new PowerUpRepair(_core) : (BasePowerUp)new PowerUpSheild(_core);
                    powerUp.Location = this.Location;
                    _core.Actors.InjectPowerUp(powerUp);
                }
            }

            _explodeSound.Play();
            _explosionAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_explosionAnimation, this);

            if (autoKill)
            {
                IsDead = true;
            }

            if (autoDelete)
            {
                QueueForDelete();
            }
        }

        public void HitExplosion()
        {
            _hitExplosionAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(_hitExplosionAnimation, this);
        }

        public List<ActorBase> Intersections()
        {
            var intersections = new List<ActorBase>();

            foreach (var intersection in _core.Actors.Collection)
            {
                if (intersection != this && intersection.Visable && intersection is not ObjTextBlock)
                {
                    if (this.Intersects(intersection))
                    {
                        intersections.Add(intersection);
                    }
                }
            }

            return intersections;
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
