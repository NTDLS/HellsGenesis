﻿using HellsGenesis.Weapons.Munitions;
using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;

namespace NebulaSiege.Weapons
{
    internal class WeaponScattershot : _WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 1;
            FireDelayMilliseconds = 25;
            Speed = 15;
            AngleVarianceDegrees = 8;
            SpeedVariancePercent = 0.10;
            RecoilAmount = 0.01;
        }

        public override _MunitionBase CreateMunition(NsPoint xyOffset, _SpriteBase targetOfLock = null)
        {
            return new MunitionScattershot(_core, this, _owner, xyOffset);
        }
    }
}
