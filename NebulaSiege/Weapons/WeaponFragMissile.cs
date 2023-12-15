using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.BaseClasses;
using NebulaSiege.Weapons.Munitions;

namespace NebulaSiege.Weapons
{
    internal class WeaponFragMissile : WeaponBase
    {
        static new string Name { get; } = "Frag Missile";
        private const string soundPath = @"Sounds\Weapons\FragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponFragMissile(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 10;
            FireDelayMilliseconds = 250;
            Speed = 11;
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
