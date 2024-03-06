﻿using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Engine.Sprites.Weapons.Munitions;
using Si.Engine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        static new string Name { get; } = "Vulcan Cannon";
        private const string soundPath = @"Sounds\Weapons\VulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(EngineCore engine, SpriteShipBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponVulcanCannon(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 2;
            FireDelayMilliseconds = 25;
            Speed = 15;
            AngleVarianceDegrees = 1.5f;
            SpeedVariancePercent = 0.05f;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionVulcanCannon(_engine, this, _owner, xyOffset);
        }
    }
}
