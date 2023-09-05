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
    internal class ActorShipBase : ActorBase
    {
        public ActorRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public ActorRadarPositionTextBlock RadarPositionText { get; protected set; }

        private readonly List<WeaponBase> _secondaryWeapons = new();
        private readonly List<WeaponBase> _primaryWeapons = new();

        public WeaponBase SelectedPrimaryWeapon { get; private set; }
        public WeaponBase SelectedSecondaryWeapon { get; private set; }

        private readonly string _assetPathlockedOnImage = @"Graphics\Weapon\Locked On.png";
        private readonly string _assetPathlockedOnSoftImage = @"Graphics\Weapon\Locked Soft.png";
        private readonly string _assetPathHitSound = @"Sounds\Ship\Object Hit.wav";
        private readonly string _assetPathshieldHit = @"Sounds\Ship\Shield Hit.wav";

        private const string _assetPathExplosionAnimation = @"Graphics\Animation\Explode\Explosion 256x256\";
        private readonly int _explosionAnimationCount = 3;
        private int _selectedExplosionAnimationIndex = 0;

        private const string _assetPathHitExplosionAnimation = @"Graphics\Animation\Explode\Hit Explosion 22x22\";
        private readonly int _hitExplosionAnimationCount = 2;
        private int _selectedHitExplosionAnimationIndex = 0;

        private const string _assetExplosionSoundPath = @"Sounds\Explode\";
        private readonly int _hitExplosionSoundCount = 2;
        private int _selectedExplosionSoundIndex = 0;

        public ActorShipBase(Core core, string assetTag = "")
            : base(core, assetTag)
        {
            _core = core;
        }

        public new void Initialize(string imagePath = null, Size? size = null)
        {
            _hitSound = _core.Audio.Get(_assetPathHitSound, 0.5f);
            _shieldHit = _core.Audio.Get(_assetPathshieldHit, 0.5f);

            _selectedExplosionSoundIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionSoundCount;
            _explodeSound = _core.Audio.Get(Path.Combine(_assetExplosionSoundPath, $"{_selectedExplosionSoundIndex}.wav"), 1.0f);

            _selectedExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _explosionAnimationCount;
            _explosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathExplosionAnimation, $"{_selectedExplosionAnimationIndex}.png"), new Size(256, 256));

            _selectedHitExplosionAnimationIndex = HgRandom.Random.Next(0, 1000) % _hitExplosionAnimationCount;
            _hitExplosionAnimation = new ActorAnimation(_core, Path.Combine(_assetPathHitExplosionAnimation, $"{_selectedHitExplosionAnimationIndex}.png"), new Size(22, 22));

            _lockedOnImage = _core.Imaging.Get(_assetPathlockedOnImage);
            _lockedOnSoftImage = _core.Imaging.Get(_assetPathlockedOnSoftImage);

            base.Initialize();
        }

        public new void Explode(bool autoKill = true, bool autoDelete = true)
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _core.Actors.Animations.CreateAt(_explosionAnimation, this);
            base.Explode(autoKill);
        }

        public virtual bool TestHit(HgPoint<double> displacementVector, BulletBase bullet)
        {
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
    }
}
