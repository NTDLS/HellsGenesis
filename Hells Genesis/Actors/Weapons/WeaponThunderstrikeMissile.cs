using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Utility;

namespace HG.Actors.Weapons
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        static new string Name { get; } = "Thunderstrike Missile";
        private const string soundPath = @"Sounds\Weapons\WeaponThunderstrikeMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore core, ActorShipBase owner)
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

        public override BulletBase CreateBullet(ActorBase lockedTarget, HgPoint xyOffset = null)
        {
            return new BulletFragMissile(_core, this, _owner, lockedTarget, xyOffset);
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
                        var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HgPoint(10, 10));
                        _core.Actors.Bullets.Create(this, _owner, pointRight);
                    }
                    else
                    {
                        var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HgPoint(10, 10));
                        _core.Actors.Bullets.Create(this, _owner, pointLeft);
                    }

                    _toggle = !_toggle;
                }

                return true;
            }

            return false;

        }
    }
}
