using HG.Actors.Ordinary;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Types.Geometry;
using HG.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace HG.Actors.BaseClasses
{
    internal class ActorShipBase : ActorBase
    {
        public ActorRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public ActorRadarPositionTextBlock RadarPositionText { get; protected set; }
        public HgTimeRenewableResources RenewableResources { get; set; } = new();

        public WeaponBase PrimaryWeapon { get; private set; }

        private readonly List<WeaponBase> _secondaryWeapons = new();
        public WeaponBase SelectedSecondaryWeapon { get; private set; }

        private readonly string _assetPathlockedOnImage = @"Graphics\Weapon\Locked On.png";
        private readonly string _assetPathlockedOnSoftImage = @"Graphics\Weapon\Locked Soft.png";
        private readonly string _assetPathHitSound = @"Sounds\Ship\Object Hit.wav";
        private readonly string _assetPathshieldHit = @"Sounds\Ship\Shield Hit.wav";

        private const string _assetPathExplosionAnimation = @"Graphics\Animation\Explode\Explosion 256x256\";
        private readonly int _explosionAnimationCount = 6;
        private int _selectedExplosionAnimationIndex = 0;

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        private const string _assetExplosionSoundPath = @"Sounds\Explode\";
        private readonly int _explosionSoundCount = 4;
        private int _selectedExplosionSoundIndex = 0;

        public ActorShipBase(Core core, string name = "")
            : base(core, name)
        {
            _core = core;
        }

        public override void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Audio.Get(_assetPathHitSound, 0.5f);
            _shieldHit = _core.Audio.Get(_assetPathshieldHit, 0.5f);

            _selectedExplosionSoundIndex = HgRandom.Random.Next(0, 1000) % _explosionSoundCount;
            _explodeSound = _core.Audio.Get(Path.Combine(_assetExplosionSoundPath, $"{_selectedExplosionSoundIndex}.wav"), 1.0f);

            _selectedExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _explosionAnimationCount;
            _explosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathExplosionAnimation, $"{_selectedExplosionAnimationIndex}.png"), new Size(256, 256));

            _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

            _lockedOnImage = _core.Imaging.Get(_assetPathlockedOnImage);
            _lockedOnSoftImage = _core.Imaging.Get(_assetPathlockedOnSoftImage);

            base.Initialize(imagePath, size);
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Actors.Animations.InsertAt(_explosionAnimation, this);
            base.Explode();
        }

        public void CreateParticlesExplosion()
        {
            _core.Actors.Particles.CreateRandomShipPartParticlesAt(this, HgRandom.RandomNumber(30, 50));
            _core.Audio.PlayRandomExplosion();
        }

        /// <summary>
        /// Allows for the testing of hits from a bullet. This is called for each movement along a bullets path.
        /// </summary>
        /// <param name="displacementVector">The background offset vector.</param>
        /// <param name="bullet">The bullet object that is being tested for.</param>
        /// <param name="hitTestPosition">The position to test for hit.</param>
        /// <returns></returns>
        public virtual bool TestHit(HgPoint displacementVector, BulletBase bullet, HgPoint hitTestPosition)
        {
            if (Intersects(hitTestPosition))
            {
                if (Hit(bullet))
                {
                    if (HullHealth <= 0)
                    {
                        Explode();
                    }
                    return true;
                }
            }
            return false;
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();
                RadarPositionText.QueueForDelete();
            }

            base.Cleanup();
        }

        #region Weapons selection and evaluation.

        public void ClearPrimaryWeapon()
        {
            PrimaryWeapon = null;
        }

        public void ClearSecondaryWeapons() => _secondaryWeapons.Clear();

        public void SetPrimaryWeapon(string weaponTypeName, int roundQuantity)
        {
            var weaponType = HgReflection.GetTypeByName(weaponTypeName);

            if (PrimaryWeapon?.GetType() == weaponType)
            {
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
            else
            {
                var weapon = HgReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _core, this });
                weapon.RoundQuantity += roundQuantity;
                PrimaryWeapon = weapon;

                if (PrimaryWeapon == null) //If there is no primary weapon selected, then default to the newly added one.
                {
                    PrimaryWeapon = weapon;
                }
            }
        }

        public void AddSecondaryWeapon(string weaponTypeName, int roundQuantity)
        {
            var weaponType = HgReflection.GetTypeByName(weaponTypeName);

            var weapon = _secondaryWeapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = HgReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _core, this });
                weapon.RoundQuantity += roundQuantity;
                _secondaryWeapons.Add(weapon);
            }

            weapon.RoundQuantity += roundQuantity;

            if (SelectedSecondaryWeapon == null)//If there is no secondary weapon selected, then default to the newly added one.
            {
                SelectedSecondaryWeapon = weapon;
            }
        }

        /// <summary>
        /// Adds a new primary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void SetPrimaryWeapon<T>(int roundQuantity) where T : WeaponBase
        {
            if (PrimaryWeapon is T)
            {
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
            else
            {
                PrimaryWeapon = HgReflection.CreateInstanceOf<T>(new object[] { _core, this });
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
        }

        /// <summary>
        /// Adds a new secondary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void AddSecondaryWeapon<T>(int roundQuantity) where T : WeaponBase
        {
            var weapon = GetSecondaryWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = HgReflection.CreateInstanceOf<T>(new object[] { _core, this });
                weapon.RoundQuantity += roundQuantity;
                _secondaryWeapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += roundQuantity;
            }

            if (SelectedSecondaryWeapon == null) //If there is no secondary weapon selected, then default to the newly added one.
            {
                SelectedSecondaryWeapon = weapon;
            }
        }

        public int TotalAvailableSecondaryWeaponRounds() => (from o in _secondaryWeapons select o.RoundQuantity).Sum();
        public int TotalSecondaryWeaponFiredRounds() => (from o in _secondaryWeapons select o.RoundsFired).Sum();

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

        public bool HasSecondaryWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasSecondaryWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool HasPrimaryWeaponAndAmmo<T>() where T : WeaponBase
        {
            if (PrimaryWeapon is T)
            {
                return PrimaryWeapon.RoundQuantity > 0;
            }
            return false;
        }

        public bool HasPrimaryWeaponAndAmmo()
        {
            return PrimaryWeapon?.RoundQuantity > 0;
        }

        public bool HasSelectedPrimaryWeaponAndAmmo()
        {
            return (PrimaryWeapon != null && PrimaryWeapon.RoundQuantity > 0);
        }

        public bool HasSelectedSecondaryWeaponAndAmmo()
        {
            return (SelectedSecondaryWeapon != null && SelectedSecondaryWeapon.RoundQuantity > 0);
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

        public WeaponBase GetSecondaryWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        public WeaponBase SelectSecondaryWeapon<T>() where T : WeaponBase
        {
            SelectedSecondaryWeapon = GetSecondaryWeaponOfType<T>();
            return SelectedSecondaryWeapon;
        }

        #endregion
    }
}
