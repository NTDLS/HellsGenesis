﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Weapons.BaseClasses;
using StrikeforceInfinity.Game.Weapons.Munitions;

namespace StrikeforceInfinity.Game.Weapons
{
    internal class WeaponFragMissile : WeaponBase
    {
        static new string Name { get; } = "Frag Missile";
        private const string soundPath = @"Sounds\Weapons\FragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(EngineCore gameCore, _SpriteShipBase owner)
            : base(gameCore, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponFragMissile(EngineCore gameCore)
            : base(gameCore, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 10;
            FireDelayMilliseconds = 250;
            Speed = 11;
            SpeedVariancePercent = 0.10;

            CanLockOn = false;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionFragMissile(_gameCore, this, _owner, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedOnObjects == null || LockedOnObjects.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new SiPoint(10, 10));
                        _gameCore.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new SiPoint(10, 10));
                        _gameCore.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }

                return true;
            }

            return false;

        }
    }
}
