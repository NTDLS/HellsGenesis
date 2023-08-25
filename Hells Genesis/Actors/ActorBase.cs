using HG.Actors.Enemies.BaseClasses;
using HG.Actors.PowerUp;
using HG.Actors.Weapons;
using HG.Actors.Weapons.Bullets;
using HG.Engine;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HG.Actors
{
    internal class ActorBase
    {
        protected Core _core;
        public bool IsBoostFading { get; set; }

        private readonly List<WeaponBase> _secondaryWeapons = new();
        private readonly List<WeaponBase> _primaryWeapons = new();

        private SolidBrush _radarDotBrush = new(Color.FromArgb(255, 255, 0, 0));
        private Image _image;

        private readonly string _assetPathlockedOnImage = @"..\..\..\Assets\Graphics\Weapon\Locked On.png";
        private Image _lockedOnImage;

        private readonly string _assetPathlockedOnSoftImage = @"..\..\..\Assets\Graphics\Weapon\Locked Soft.png";
        private Image _lockedOnSoftImage;

        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private readonly int _MillisecondsBetweenHits = 200;

        private readonly string _assetPathHitSound = @"..\..\..\Assets\Sounds\Ship\Object Hit.wav";
        private AudioClip _hitSound;

        private readonly string _assetPathshieldHit = @"..\..\..\Assets\Sounds\Ship\Shield Hit.wav";
        private AudioClip _shieldHit;

        private ActorAnimation _explosionAnimation;
        private const string _assetPathExplosionAnimation = @"..\..\..\Assets\Graphics\Animation\Explode\Explosion 256x256\";
        private readonly int _explosionAnimationCount = 3;
        private int _selectedExplosionAnimationIndex = 0;

        private ActorAnimation _hitExplosionAnimation;
        private const string _assetPathHitExplosionAnimation = @"..\..\..\Assets\Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        private AudioClip _explodeSound;
        private const string _assetExplosionSoundPath = @"..\..\..\Assets\Sounds\Explode\";
        private readonly int _hitExplosionSoundCount = 2;
        private int _selectedHitExplosionSoundIndex = 0;

        #region Properties.

        public string AssetTag { get; set; }
        public Guid UID { get; private set; } = Guid.NewGuid();
        public List<ActorAttachment> Attachments { get; private set; } = new();
        public HgPoint<int> RadarDotSize { get; set; } = new HgPoint<int>(4, 4);
        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        public bool Highlight { get; set; } = false;
        public HgRotationMode RotationMode { get; set; }
        public WeaponBase SelectedPrimaryWeapon { get; private set; }
        public WeaponBase SelectedSecondaryWeapon { get; private set; }
        public int HitPoints { get; private set; } = 0;
        public int ShieldPoints { get; private set; } = 0;

        #region Events.

        public delegate void HitEvent(ActorBase sender, HgDamageType damageType);
        public event HitEvent OnHit;

        public delegate void QueuedForDeleteEvent(ActorBase sender);
        public event QueuedForDeleteEvent OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(ActorBase sender);
        public event VisibilityChangedEvent OnVisibilityChanged;


        public delegate void ExplodeEvent(ActorBase sender);
        public event ExplodeEvent OnExplode;

        #endregion


        private HgVelocity<double> _velocity;
        public HgVelocity<double> Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
                _velocity.OnThrottleChange += _velocity_OnThrottleChange;
            }
        }

        private void _velocity_OnThrottleChange(HgVelocity<double> sender)
        {
            VelocityChanged();
        }

        public bool IsDead { get; set; } = false;

        public void SetHitPoints(int points)
        {
            HitPoints = 0;
            AddHitPoints(points);
        }

        /// <summary>
        /// Creates a new actor, adds it to the actor collection but also adds it to the collection of another actors children for automatic cleanup when parent is destroyed. 
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public ActorAttachment Attach(string imagePath, bool takesDamage = false, int hitPoints = 1)
        {
            var attachment = _core.Actors.AddNewActorAttachment(imagePath, null, UID.ToString() + $"_Attachment_{Attachments.Count}");
            attachment.TakesDamage = takesDamage;
            attachment.SetHitPoints(hitPoints);
            Attachments.Add(attachment);
            return attachment;
        }

        public void AddHitPoints(int pointsToAdd)
        {
            if (HitPoints + pointsToAdd > _core.Settings.MaxHitpoints)
            {
                pointsToAdd = _core.Settings.MaxHitpoints - (HitPoints + pointsToAdd);
            }

            /*
            if (this is ObjPlayer)
            {
                var player = this as ObjPlayer;

                if (HitPoints < _core.Settings.MaxShieldPoints && HitPoints + pointsToAdd >= _core.Settings.MaxShieldPoints)
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
            if (ShieldPoints + pointsToAdd > _core.Settings.MaxShieldPoints)
            {
                pointsToAdd = _core.Settings.MaxShieldPoints - (ShieldPoints + pointsToAdd);
            }

            if (this is ActorPlayer)
            {
                var player = this as ActorPlayer;

                if (ShieldPoints < _core.Settings.MaxShieldPoints && ShieldPoints + pointsToAdd >= _core.Settings.MaxShieldPoints)
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
                    _core.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        public void QueueForDelete()
        {
            Visable = false;
            if (_readyForDeletion == false)
            {
                VisibilityChanged();
            }

            OnQueuedForDelete?.Invoke(this);

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
                return _core.Display.CurrentScaledScreenBounds.IntersectsWith(Bounds);
            }
        }

        private HgPoint<double> _location = new HgPoint<double>();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public HgPoint<double> Location
        {
            get
            {
                return new HgPoint<double>(_location);
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
                PositionChanged();
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
                PositionChanged();
                Invalidate();
            }
        }

        public HgPoint<double> LocationCenter
        {
            get
            {
                return new HgPoint<double>(_location.X - Size.Width / 2.0, _location.Y - Size.Height / 2.0);
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

        public RectangleF VisibleBounds
        {
            get
            {
                return new Rectangle((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);
            }
        }

        public RectangleF Bounds
        {
            get
            {
                return new RectangleF((float)_location.X, (float)_location.Y, Size.Width, Size.Height);
            }
        }

        public Rectangle BoundsI
        {
            get
            {
                return new Rectangle((int)_location.X, (int)_location.Y, Size.Width, Size.Height);
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

                    OnVisibilityChanged?.Invoke(this);

                    /*
                    var invalidRect = new Rectangle(
                        (int)(_location.X - _size.Width / 2.0),
                        (int)(_location.Y - _size.Height / 2.0),
                        _size.Width, _size.Height);
                    _core.Display.DrawingSurface.Invalidate(invalidRect);
                    */

                    VisibilityChanged();
                }
            }
        }

        #endregion

        public ActorBase(Core core, string assetTag = "")
        {
            _core = core;
            AssetTag = assetTag;
            RotationMode = HgRotationMode.Upsize;
            Velocity = new HgVelocity<double>();
            Velocity.MaxRotationSpeed = _core.Settings.MaxRotationSpeed;
            Highlight = _core.Settings.HighlightAllActors;
        }

        public void Initialize(string imagePath = null, Size? size = null,
            string explosionAnimationFile = null, Size? explosionAnimationDimensions = null, string explosionSoundFile = null,
            string hitAnimationFile = null, Size? hitAnimationFileDimensions = null, string hitSoundFile = null, string shieldHitSoundFile = null)
        {
            if (hitSoundFile == null)
                _hitSound = _core.Audio.Get(_assetPathHitSound, 0.5f);
            else _hitSound = _core.Audio.Get(hitSoundFile, 0.5f);

            if (shieldHitSoundFile == null)
                _shieldHit = _core.Audio.Get(_assetPathshieldHit, 0.5f);
            else _shieldHit = _core.Audio.Get(shieldHitSoundFile, 0.5f);

            if (explosionSoundFile == null)
            {
                _selectedHitExplosionSoundIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionSoundCount;
                _explodeSound = _core.Audio.Get(Path.Combine(_assetExplosionSoundPath, $"{_selectedHitExplosionSoundIndex}.wav"), 1.0f);
            }
            else _explodeSound = _core.Audio.Get(explosionSoundFile, 1.0f);

            if (explosionAnimationFile == null)
            {
                _selectedExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _explosionAnimationCount;
                _explosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathExplosionAnimation, $"{_selectedExplosionAnimationIndex}.png"), new Size(256, 256));
            }
            else _explosionAnimation = new ActorAnimation(_core, explosionAnimationFile, (Size)explosionAnimationDimensions);

            _lockedOnImage = _core.Imaging.Get(_assetPathlockedOnImage);
            _lockedOnSoftImage = _core.Imaging.Get(_assetPathlockedOnSoftImage);

            if (hitAnimationFile == null)
            {
                _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
                _hitExplosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));
            }
            else
            {
                _hitExplosionAnimation = new ActorAnimation(_core, hitAnimationFile, (Size)hitAnimationFileDimensions);
            }

            if (imagePath != null)
            {
                SetImage(imagePath, size);
            }

            VisibilityChanged();
        }

        public void ApplyMotion(HgPoint<double> displacementVector)
        {
            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
        }

        public void ClearPrimaryWeapons()
        {
            _primaryWeapons.Clear();
        }

        public void ClearSecondaryWeapons()
        {
            _secondaryWeapons.Clear();
        }

        public void AddPrimaryWeapon(WeaponBase weapon)
        {
            var existing = GetPrimaryWeaponOfType(weapon.GetType());
            if (existing == null)
            {
                weapon.SetOwner(this);
                _primaryWeapons.Add(weapon);
            }
            else
            {
                existing.RoundQuantity += weapon.RoundQuantity;
            }
        }

        public void AddSecondaryWeapon(WeaponBase weapon)
        {
            var existing = GetSecondaryWeaponOfType(weapon.GetType());
            if (existing == null)
            {
                weapon.SetOwner(this);
                _secondaryWeapons.Add(weapon);
            }
            else
            {
                existing.RoundQuantity += weapon.RoundQuantity;
            }
        }

        public int TotalAvailableSecondaryWeaponRounds()
        {
            return (from o in _secondaryWeapons select o.RoundQuantity).Sum();
        }

        public int TotalSecondaryWeaponFiredRounds()
        {
            return (from o in _secondaryWeapons select o.RoundsFired).Sum();
        }

        public WeaponBase SelectPreviousAvailableUsableSecondaryWeapon()
        {
            WeaponBase previousWeapon = null;

            foreach (var weapon in _secondaryWeapons)
            {
                if (weapon == SelectedSecondaryWeapon)
                {
                    if (previousWeapon == null)
                    {
                        return SelectLastAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the end.
                    }
                    SelectedSecondaryWeapon = previousWeapon;
                    return previousWeapon;
                }

                previousWeapon = weapon;
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public WeaponBase SelectNextAvailableUsableSecondaryWeapon()
        {
            bool selectNextWeapon = false;

            foreach (var weapon in _secondaryWeapons)
            {
                if (selectNextWeapon)
                {
                    SelectedSecondaryWeapon = weapon;
                    return weapon;
                }

                if (weapon == SelectedSecondaryWeapon) //Find the current weapon in the collection;
                {
                    selectNextWeapon = true;
                }
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public bool HasSecondaryWeapon(Type weaponType)
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == weaponType select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasSecondaryWeaponAndAmmo(Type weaponType)
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == weaponType select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public WeaponBase SelectLastAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in _secondaryWeapons where o.RoundQuantity > 0 select o).LastOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase SelectFirstAvailableUsablePrimaryWeapon()
        {
            var existingWeapon = (from o in _primaryWeapons where o.RoundQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                SelectedPrimaryWeapon = existingWeapon;
            }
            else
            {
                SelectedPrimaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase SelectFirstAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in _secondaryWeapons where o.RoundQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase GetPrimaryWeaponOfType(Type weaponType)
        {
            return (from o in _primaryWeapons where o.GetType() == weaponType select o).FirstOrDefault();
        }

        public WeaponBase GetSecondaryWeaponOfType(Type weaponType)
        {
            return (from o in _secondaryWeapons where o.GetType() == weaponType select o).FirstOrDefault();
        }

        public WeaponBase SelectPrimaryWeapon(Type weaponType)
        {
            SelectedPrimaryWeapon = GetPrimaryWeaponOfType(weaponType);
            return SelectedPrimaryWeapon;
        }

        public WeaponBase SelectSecondaryWeapon(Type weaponType)
        {
            SelectedSecondaryWeapon = GetSecondaryWeaponOfType(weaponType);
            return SelectedSecondaryWeapon;
        }

        public void SetImage(Image image, Size? size = null)
        {
            _image = image;

            if (size != null)
            {
                _image = HgGraphics.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public void SetImage(string imagePath, Size? size = null)
        {
            _image = _core.Imaging.Get(imagePath);

            if (size != null)
            {
                _image = HgGraphics.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }

            _size = new Size(_image.Size.Width, _image.Size.Height);
        }

        public Image GetImage()
        {
            return _image;
        }

        public void Invalidate()
        {
            /*
            var invalidRect = new Rectangle(
                (int)(_location.X - _size.Width / 2.0),
                (int)(_location.Y - _size.Height / 2.0),
                _size.Width, _size.Height);
            _core.Display.DrawingSurface.Invalidate(invalidRect);
            */
        }

        public bool Intersects(ActorBase otherObject)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
            {
                return Bounds.IntersectsWith(otherObject.Bounds);
            }
            return false;
        }

        /// <summary>
        /// Allows for intersect detection with larger/smaller "hit box".
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="sizeAdjust"></param>
        /// <returns></returns>
        public bool Intersects(ActorBase otherObject, HgPoint<double> sizeAdjust)
        {
            if (Visable && otherObject.Visable && !ReadyForDeletion && !otherObject.ReadyForDeletion)
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

        public bool Intersects(HgPoint<double> location, HgPoint<double> size)
        {
            var alteredHitBox = new RectangleF(
                (float)location.X,
                (float)location.Y,
                (float)size.X,
                (float)size.Y
                );

            return VisibleBounds.IntersectsWith(alteredHitBox);
        }

        public bool Intersects(ActorBase with, int slop = 0)
        {
            var alteredHitBox = new RectangleF(
                (float)(with.Bounds.X - slop),
                (float)(with.Bounds.Y - slop),
                with.Size.Width + slop * 2, with.Size.Height + slop * 2);

            return Bounds.IntersectsWith(alteredHitBox);
        }

        #region Actions.

        /// <summary>
        /// Subtract from the objects hitpoints.
        /// </summary>
        /// <returns></returns>
        public bool Hit(int damage, bool autoKill = true, bool autoDelete = true)
        {
            bool result = (DateTime.Now - _lastHit).TotalMilliseconds > _MillisecondsBetweenHits;
            if (result)
            {
                _lastHit = DateTime.Now;

                if (ShieldPoints > 0)
                {
                    _shieldHit.Play();
                    damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                    damage = damage < 1 ? 1 : damage;
                    ShieldPoints -= damage > ShieldPoints ? ShieldPoints : damage; //No need to go negative with the damage.

                    OnHit?.Invoke(this, HgDamageType.Shield);

                    if (this is ActorPlayer)
                    {
                        var player = this as ActorPlayer;
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

                    OnHit?.Invoke(this, HgDamageType.Hull);

                    if (this is ActorPlayer)
                    {
                        var player = this as ActorPlayer;
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
                        Explode(autoKill, autoDelete);
                    }
                }
            }

            return result;
        }

        public bool Hit(BulletBase bullet, bool autoKill = true, bool autoDelete = true)
        {
            if (bullet != null)
            {
                return Hit(bullet.Weapon.Damage, autoKill, autoDelete);
            }
            return false;
        }

        public void Rotate(double degrees)
        {
            Velocity.Angle.Degrees += degrees;
            Invalidate();
            RotationChanged();
        }

        public void MoveInDirectionOf(HgPoint<double> location, double? speed = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, location);
            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(ActorBase obj, double? speed = null)
        {
            Velocity.Angle.Degrees = HgPoint<double>.AngleTo(Location, obj.Location);

            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        /// <summary>
        /// Calculated the difference in heading angle from one object to get to another.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        [Obsolete("This function returns the angle in 0-360 degress which is almost impossible to work with, use DeltaAngle() instead.")]
        public double DeltaAngle360(ActorBase atObj)
        {
            return HgMath.DeltaAngle360(this, atObj);
        }

        public double DeltaAngle(ActorBase atObj)
        {
            return HgMath.DeltaAngle(this, atObj);
        }

        /// <summary>
        /// Calculates the angle in degrees of one objects location to another location.
        /// </summary>
        /// <param name="atObj"></param>
        /// <returns></returns>
        public double AngleTo(ActorBase atObj)
        {
            return HgMath.AngleTo(this, atObj);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance, offsetAngle);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees, maxDistance);
        }

        public bool IsPointingAt(ActorBase atObj, double toleranceDegrees)
        {
            return HgMath.IsPointingAt(this, atObj, toleranceDegrees);
        }

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees)
        {
            return HgMath.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return HgMath.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(ActorBase to)
        {
            return HgPoint<double>.DistanceTo(Location, to.Location);
        }

        public double DistanceTo(HgPoint<double> to)
        {
            return HgPoint<double>.DistanceTo(Location, to);
        }

        public void Explode(bool autoKill = true, bool autoDelete = true)
        {
            if (this is EnemyBase)
            {
                _core.Player.Actor.Score += (this as EnemyBase).ScorePoints;

                //If the type of explosion is an enemy then maybe spawn a powerup.
                if (HgRandom.ChanceIn(5))
                {
                    PowerUpBase powerUp = HgRandom.FlipCoin() ? new PowerUpRepair(_core) : new PowerUpSheild(_core);
                    powerUp.Location = Location;
                    _core.Actors.Powerups.Insert(powerUp);
                }
            }

            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Actors.Animations.CreateAt(_explosionAnimation, this);

            if (autoKill)
            {
                IsDead = true;
            }

            if (autoDelete)
            {
                QueueForDelete();
            }

            OnExplode?.Invoke(this);
        }

        public void HitExplosion()
        {
            _hitExplosionAnimation.Reset();
            _core.Actors.Animations.CreateAt(_hitExplosionAnimation, this);
        }

        public List<ActorBase> Intersections()
        {
            var intersections = new List<ActorBase>();

            foreach (var intersection in _core.Actors.Collection)
            {
                if (intersection != this && intersection.Visable && intersection is not ActorTextBlock)
                {
                    if (Intersects(intersection))
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
            Invalidate();

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        public virtual void VelocityChanged()
        {
        }

        public virtual void VisibilityChanged()
        {
        }

        public virtual void PositionChanged()
        {
        }

        public virtual void RotationChanged()
        {
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

                if (Highlight)
                {
                    using (var pen = new Pen(Color.Red, 3))
                    {
                        var rect = new Rectangle((int)(_location.X - Size.Width / 2.0), (int)(_location.Y - Size.Height / 2.0), Size.Width, Size.Height);
                        dc.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        public Color RadarDotColor
        {
            set
            {
                _radarDotBrush = new SolidBrush(value);
            }
        }

        public void RenderRadar(Graphics dc, HgPoint<double> scale, HgPoint<double> offset)
        {
            if (_isVisible && _image != null)
            {
                double x = (int)(X * scale.X + offset.X - RadarDotSize.X / 2.0);
                double y = (int)(Y * scale.Y + offset.Y - RadarDotSize.Y / 2.0);

                dc.FillEllipse(_radarDotBrush, (int)x, (int)y, RadarDotSize.X, RadarDotSize.Y);
            }
        }

        private void DrawImage(Graphics dc, Image rawImage, double? angleInDegrees = null)
        {
            double angle = (double)(angleInDegrees == null ? Velocity.Angle.Degrees : angleInDegrees);

            var bitmap = new Bitmap(rawImage);

            if (angle != 0 && RotationMode != HgRotationMode.None)
            {
                if (RotationMode == HgRotationMode.Upsize) //Very expensize
                {
                    var image = HgGraphics.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    var rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == HgRotationMode.Clip) //Much less expensive.
                {
                    var image = HgGraphics.RotateImageWithClipping(bitmap, angle, Color.Transparent);
                    var rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);

                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
            }
            else //Almost free.
            {
                Rectangle rect = new Rectangle((int)(_location.X - bitmap.Width / 2.0), (int)(_location.Y - bitmap.Height / 2.0), bitmap.Width, bitmap.Height);
                dc.DrawImage(bitmap, rect);
            }
        }

        #endregion
    }
}
