﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition
{
    internal class MunitionPhotonTorpedo : EnergyMunitionBase
    {
        private const string imagePath = @"Graphics\Weapon\PhotonTorpedo.png";

        public MunitionPhotonTorpedo(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, location, angle)
        {
            Initialize(imagePath);
        }
    }
}
