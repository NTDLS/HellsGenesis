using HG.Actors.Objects.Weapons.Bullets;
using HG.Engine;
using HG.Types;

namespace HG.Actors.Objects.Weapons
{
    internal class WeaponPrecisionGuidedFragMissile : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponGuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPrecisionGuidedFragMissile(Core core)
            : base(core, "Precision Guided Frag Missile", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 8;
            FireDelayMilliseconds = 800;
            Speed = 10;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 1000;
            MaxLocks = 1;
            MaxLockOnAngle = 45;
            ExplodesOnImpact = true;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HGPoint<double> xyOffset = null)
        {
            return new BulletGuidedFragMissile(_core, this, _owner, lockedTarget, xyOffset);
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
                        var pointRight = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HGPoint<double>(10, 10));
                        _core.Actors.Bullets.Create(this, _owner, pointRight);
                    }
                    else
                    {
                        var pointLeft = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HGPoint<double>(10, 10));
                        _core.Actors.Bullets.Create(this, _owner, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var lockedOn in LockedOnObjects)
                    {
                        if (_toggle)
                        {
                            var pointRight = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new HGPoint<double>(10, 10));
                            _core.Actors.Bullets.CreateLocked(this, _owner, lockedOn, pointRight);
                        }
                        else
                        {
                            var pointLeft = HGMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new HGPoint<double>(10, 10));
                            _core.Actors.Bullets.CreateLocked(this, _owner, lockedOn, pointLeft);
                        }
                        _toggle = !_toggle;
                    }
                }


                return true;
            }
            return false;

        }
    }
}
