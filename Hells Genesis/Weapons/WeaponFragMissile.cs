using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using HG.Utility;
using HG.Weapons.BaseClasses;
using HG.Weapons.Bullets;
using HG.Weapons.Bullets.BaseClasses;

namespace HG.Weapons
{
    internal class WeaponFragMissile : WeaponBase
    {
        static new string Name { get; } = "Frag Missile";
        private const string soundPath = @"Sounds\Weapons\WeaponFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(EngineCore core, ActorShipBase owner)
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
