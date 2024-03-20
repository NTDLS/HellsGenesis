﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionVulcanCannon : ProjectileMunitionBase
    {
        private const string imagePath = @"Sprites\Weapon\VulcanCannon.png";

        public MunitionVulcanCannon(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, SiPoint location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
        }
    }
}
