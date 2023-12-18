using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Weapons.BaseClasses;
using NebulaSiege.Game.Weapons.Munitions;

namespace NebulaSiege.Game.Weapons
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        static new string Name { get; } = "Thunderstrike Missile";
        private const string soundPath = @"Sounds\Weapons\ThunderstrikeMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponThunderstrikeMissile(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 20;
            FireDelayMilliseconds = 250;
            Speed = 15;
            SpeedVariancePercent = 0.10;

            CanLockOn = false;
            ExplodesOnImpact = true;
        }

        public override MunitionBase CreateMunition(NsPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionFragMissile(_core, this, _owner, xyOffset);
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
                        var pointRight = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new NsPoint(10, 10));
                        _core.Sprites.Munitions.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new NsPoint(10, 10));
                        _core.Sprites.Munitions.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }

                return true;
            }

            return false;

        }
    }
}
