using HellsGenesis.Weapons.Munitions;
using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Utility;
using NebulaSiege.Weapons;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace NebulaSiege.Sprites
{
    internal class _SpriteShipBase : _SpriteBase
    {
        public SpriteRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public SpriteRadarPositionTextBlock RadarPositionText { get; protected set; }
        public NsTimeRenewableResources RenewableResources { get; set; } = new();

        public _WeaponBase PrimaryWeapon { get; private set; }

        private readonly List<_WeaponBase> _secondaryWeapons = new();
        public _WeaponBase SelectedSecondaryWeapon { get; private set; }

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

        public _SpriteShipBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }

        public override void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Assets.GetAudio(_assetPathHitSound, 0.5f);
            _shieldHit = _core.Assets.GetAudio(_assetPathshieldHit, 0.5f);

            _selectedExplosionSoundIndex = HgRandom.Generator.Next(0, 1000) % _explosionSoundCount;
            _explodeSound = _core.Assets.GetAudio(Path.Combine(_assetExplosionSoundPath, $"{_selectedExplosionSoundIndex}.wav"), 1.0f);

            _selectedExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _explosionAnimationCount;
            _explosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathExplosionAnimation, $"{_selectedExplosionAnimationIndex}.png"), new Size(256, 256));

            _selectedHitExplosionAnimationIndex = HgRandom.Generator.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new SpriteAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

            _lockedOnImage = _core.Assets.GetBitmap(_assetPathlockedOnImage);
            _lockedOnSoftImage = _core.Assets.GetBitmap(_assetPathlockedOnSoftImage);

            base.Initialize(imagePath, size);
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Sprites.Animations.InsertAt(_explosionAnimation, this);
            base.Explode();
        }

        public void CreateParticlesExplosion()
        {
            _core.Sprites.Particles.CreateRandomShipPartParticlesAt(this, HgRandom.Between(30, 50));
            _core.Audio.PlayRandomExplosion();
        }

        /// <summary>
        /// Allows for the testing of hits from a munition. This is called for each movement along a munitions path.
        /// </summary>
        /// <param name="displacementVector">The background offset vector.</param>
        /// <param name="munition">The munition object that is being tested for.</param>
        /// <param name="hitTestPosition">The position to test for hit.</param>
        /// <returns></returns>
        public virtual bool TryMunitionHit(NsPoint displacementVector, _MunitionBase munition, NsPoint hitTestPosition)
        {
            if (Intersects(hitTestPosition))
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
            var weaponType = NsReflection.GetTypeByName(weaponTypeName);

            if (PrimaryWeapon?.GetType() == weaponType)
            {
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
            else
            {
                var weapon = NsReflection.CreateInstanceFromType<_WeaponBase>(weaponType, new object[] { _core, this });
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
            var weaponType = NsReflection.GetTypeByName(weaponTypeName);

            var weapon = _secondaryWeapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = NsReflection.CreateInstanceFromType<_WeaponBase>(weaponType, new object[] { _core, this });
                weapon.RoundQuantity += roundQuantity;
                _secondaryWeapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += roundQuantity;
            }

            if (SelectedSecondaryWeapon == null)//If there is no secondary weapon selected, then default to the newly added one.
            {
                SelectedSecondaryWeapon = weapon;
            }
        }

        /// <summary>
        /// Adds a new primary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void SetPrimaryWeapon<T>(int roundQuantity) where T : _WeaponBase
        {
            if (PrimaryWeapon is T)
            {
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
            else
            {
                PrimaryWeapon = NsReflection.CreateInstanceOf<T>(new object[] { _core, this });
                PrimaryWeapon.RoundQuantity += roundQuantity;
            }
        }

        /// <summary>
        /// Adds a new secondary weapon or adds its ammo to the current of its type.
        /// </summary>
        /// <param name="weapon"></param>
        public void AddSecondaryWeapon<T>(int roundQuantity) where T : _WeaponBase
        {
            var weapon = GetSecondaryWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = NsReflection.CreateInstanceOf<T>(new object[] { _core, this });
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

        public _WeaponBase SelectPreviousAvailableUsableSecondaryWeapon()
        {
            _WeaponBase previousWeapon = null;

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

        public _WeaponBase SelectNextAvailableUsableSecondaryWeapon()
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

        public bool HasSecondaryWeapon<T>() where T : _WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasSecondaryWeaponAndAmmo<T>() where T : _WeaponBase
        {
            var existingWeapon = (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool HasPrimaryWeaponAndAmmo<T>() where T : _WeaponBase
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
            return PrimaryWeapon != null && PrimaryWeapon.RoundQuantity > 0;
        }

        public bool HasSelectedSecondaryWeaponAndAmmo()
        {
            return SelectedSecondaryWeapon != null && SelectedSecondaryWeapon.RoundQuantity > 0;
        }

        public _WeaponBase SelectLastAvailableUsableSecondaryWeapon()
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

        public _WeaponBase SelectFirstAvailableUsableSecondaryWeapon()
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

        public _WeaponBase GetSecondaryWeaponOfType<T>() where T : _WeaponBase
        {
            return (from o in _secondaryWeapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        public _WeaponBase SelectSecondaryWeapon<T>() where T : _WeaponBase
        {
            SelectedSecondaryWeapon = GetSecondaryWeaponOfType<T>();
            return SelectedSecondaryWeapon;
        }

        #endregion
    }
}
