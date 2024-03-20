﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        static string Name { get; } = "Thunderstrike Missile";
        private const string soundPath = @"Sounds\Weapons\ThunderstrikeMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore engine, SpriteInteractiveBase owner)
            : base(engine, owner, Name, soundPath, soundVolumne)
        {
        }

        public WeaponThunderstrikeMissile(EngineCore engine)
            : base(engine, Name, soundPath, soundVolumne)
        {
        }

        public override MunitionBase CreateMunition(SiPoint location = null, SpriteInteractiveBase lockedTarget = null)
            => new MunitionFragMissile(_engine, this, Owner, location);

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedTargets == null || LockedTargets.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Add(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = Owner.Location + SiPoint.PointFromAngleAtDistance360(Owner.Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(10, 10));
                        _engine.Sprites.Munitions.Add(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }

                return true;
            }

            return false;

        }
    }
}
