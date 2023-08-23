using AI2D.Actors.Objects.Enemies;
using AI2D.Actors.Objects.PowerUp;
using AI2D.Actors.Objects.Weapons;
using AI2D.Actors.Objects.Weapons.Bullets;
using AI2D.Engine;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.Objects
{
    internal class ActorBase
    {
        /// <summary>
        /// A user defined name for the actor.
        /// </summary>
        public string Tag { get; set; }
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;

        public List<ActorAttachment> Attachments { get; private set; } = new List<ActorAttachment>();

        private SolidBrush _radarDotBrush = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
        private Image _image;
        private Image _lockedOnImage;
        private Image _lockedOnSoftImage;
        private ActorAnimation _explosionAnimation;
        private AudioClip _explodeSound;
        private DateTime _lastHit = DateTime.Now.AddMinutes(-5);
        private readonly int _MillisecondsBetweenHits = 200;
        private AudioClip _hitSound;
        private AudioClip _shieldHit;
        private readonly List<WeaponBase> _secondaryWeapons = new List<WeaponBase>();
        private readonly List<WeaponBase> _primaryWeapons = new List<WeaponBase>();
        private ActorAnimation _hitExplosionAnimation { get; set; }
        public Point<int> RadarDotSize { get; set; } = new Point<int>(4, 4);
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

        public bool Highlight { get; set; } = false;
        public RotationMode RotationMode { get; set; }
        public WeaponBase SelectedPrimaryWeapon { get; private set; }
        public WeaponBase SelectedSecondaryWeapon { get; private set; }
        public int HitPoints { get; private set; } = 0;
        public int ShieldPoints { get; private set; } = 0;

        private Velocity<double> _velocity;
        public Velocity<double> Velocity
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

        private void _velocity_OnThrottleChange(Velocity<double> sender)
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

        private Point<double> _location = new Point<double>();

        /// <summary>
        /// Do not modify this location, it will not have any affect.
        /// </summary>
        public Point<double> Location
        {
            get
            {
                return new Point<double>(_location);
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

        public Point<double> LocationCenter
        {
            get
            {
                return new Point<double>(_location.X - Size.Width / 2.0, _location.Y - Size.Height / 2.0);
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

                    var invalidRect = new Rectangle(
                        (int)(_location.X - _size.Width / 2.0),
                        (int)(_location.Y - _size.Height / 2.0),
                        _size.Width, _size.Height);

                    //_core.Display.DrawingSurface.Invalidate(invalidRect);

                    VisibilityChanged();
                }
            }
        }

        #endregion

        public ActorBase(Core core, string tag = "")
        {
            _core = core;
            Tag = tag;
            RotationMode = RotationMode.Upsize;
            Velocity = new Velocity<double>();
            Velocity.MaxRotationSpeed = _core.Settings.MaxRotationSpeed;
            Highlight = _core.Settings.HighlightAllActors;
        }

        public void Initialize(string imagePath = null, Size? size = null,
            string explosionAnimationFile = null, Size? explosionAnimationDimensions = null, string explosionSoundFile = null,
            string hitAnimationFile = null, Size? hitAnimationFileDimensions = null, string hitSoundFile = null, string shieldHitSoundFile = null)
        {
            if (hitSoundFile == null)
            {
                _hitSound = _core.Audio.Get(@"..\..\..\Assets\Sounds\Ship\Object Hit.wav", 0.5f);
            }
            else
            {
                _hitSound = _core.Audio.Get(hitSoundFile, 0.5f);
            }

            if (shieldHitSoundFile == null)
            {
                _shieldHit = _core.Audio.Get(@"..\..\..\Assets\Sounds\Ship\Shield Hit.wav", 0.5f);
            }
            else
            {
                _shieldHit = _core.Audio.Get(shieldHitSoundFile, 0.5f);
            }

            if (explosionSoundFile == null)
            {
                int _explosionSoundIndex = Utility.RandomNumber(0, _assetExplosionSoundFiles.Count());
                _explodeSound = _core.Audio.Get(_assetExplosionSoundPath + _assetExplosionSoundFiles[_explosionSoundIndex], 1.0f);
            }
            else
            {
                _explodeSound = _core.Audio.Get(explosionSoundFile, 1.0f);
            }

            if (explosionAnimationFile == null)
            {
                int _explosionImageIndex = Utility.RandomNumber(0, _assetExplosionAnimationFiles.Count());
                _explosionAnimation = new ActorAnimation(_core, _assetExplosionAnimationPath + _assetExplosionAnimationFiles[_explosionImageIndex], new Size(256, 256));
            }
            else
            {
                _explosionAnimation = new ActorAnimation(_core, explosionAnimationFile, (Size)explosionAnimationDimensions);
            }

            _lockedOnImage = _core.Imaging.Get(@"..\..\..\Assets\Graphics\Weapon\Locked On.png");
            _lockedOnSoftImage = _core.Imaging.Get(@"..\..\..\Assets\Graphics\Weapon\Locked Soft.png");

            if (hitAnimationFile == null)
            {
                int _hitExplosionImageIndex = Utility.RandomNumber(0, _assetHitExplosionAnimationFiles.Count());
                _hitExplosionAnimation = new ActorAnimation(_core, _assetHitExplosionAnimationPath + _assetHitExplosionAnimationFiles[_hitExplosionImageIndex], new Size(22, 22));
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
                _image = Utility.ResizeImage(_image, ((Size)size).Width, ((Size)size).Height);
            }
            _size = new Size(_image.Size.Width, _image.Size.Height);
            Invalidate();
        }

        public void SetImage(string imagePath, Size? size = null)
        {
            _image = _core.Imaging.Get(imagePath);

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
                (int)(_location.X - _size.Width / 2.0),
                (int)(_location.Y - _size.Height / 2.0),
                _size.Width, _size.Height);
            //_core.Display.DrawingSurface.Invalidate(invalidRect);
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
        public bool Intersects(ActorBase otherObject, Point<double> sizeAdjust)
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

        public bool Intersects(Point<double> location, Point<double> size)
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

        public void MoveInDirectionOf(Point<double> location, double? speed = null)
        {
            Velocity.Angle.Degrees = Point<double>.AngleTo(Location, location);
            if (speed != null)
            {
                Velocity.MaxSpeed = (double)speed;
            }
        }

        public void MoveInDirectionOf(ActorBase obj, double? speed = null)
        {
            Velocity.Angle.Degrees = Point<double>.AngleTo(Location, obj.Location);

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

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees)
        {
            return Utility.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public bool IsPointingAway(ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return Utility.IsPointingAway(this, atObj, toleranceDegrees);
        }

        public double DistanceTo(ActorBase to)
        {
            return Point<double>.DistanceTo(Location, to.Location);
        }

        public double DistanceTo(Point<double> to)
        {
            return Point<double>.DistanceTo(Location, to);
        }

        public void Explode(bool autoKill = true, bool autoDelete = true)
        {
            if (this is EnemyBase)
            {
                _core.Actors.Player.Score += (this as EnemyBase).ScorePoints;

                //If the type of explosion is an enemy then maybe spawn a powerup.
                if (Utility.ChanceIn(5))
                {
                    PowerUpBase powerUp = Utility.FlipCoin() ? new PowerUpRepair(_core) : new PowerUpSheild(_core);
                    powerUp.Location = Location;
                    _core.Actors.Powerups.Insert(powerUp);
                }
            }

            _explodeSound.Play();
            _explosionAnimation.Reset();
            _core.Actors.Animations.CreateAt(_explosionAnimation, this);

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

        public void RenderRadar(Graphics dc, Point<double> scale, Point<double> offset)
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

            if (angle != 0 && RotationMode != RotationMode.None)
            {
                if (RotationMode == RotationMode.Upsize) //Very expensize
                {
                    var image = Utility.RotateImageWithUpsize(bitmap, angle, Color.Transparent);
                    var rect = new Rectangle((int)(_location.X - image.Width / 2.0), (int)(_location.Y - image.Height / 2.0), image.Width, image.Height);
                    dc.DrawImage(image, rect);
                    _size.Height = image.Height;
                    _size.Width = image.Width;
                }
                else if (RotationMode == RotationMode.Clip) //Much less expensive.
                {
                    var image = Utility.RotateImageWithClipping(bitmap, angle, Color.Transparent);
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
