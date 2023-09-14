﻿using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Engine;

namespace HG.Actors.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(Core core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponVulcanCannon(Core core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            RoundQuantity = 500;
            Damage = 2;
            FireDelayMilliseconds = 100;
            Speed = 20;
            AngleVariancePercent = 0.05;
            SpeedVariancePercent = 0.05;
            RecoilAmount = 0.25;
        }
    }
}
