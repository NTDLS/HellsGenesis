﻿using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;

namespace AI2D.Actors.Bullets
{
    internal class BulletPulseMeson : BulletBase
    {
        private const string imagePath = @"..\..\..\Assets\Graphics\Weapon\BulletPulseMeson.png";

        public BulletPulseMeson(Core core, WeaponBase weapon, ActorBase firedFrom,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, lockedTarget, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
