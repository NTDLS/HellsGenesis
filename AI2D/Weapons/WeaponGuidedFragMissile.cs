using AI2D.Engine;
using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Actors.Enemies;
using AI2D.Types;
using System.Collections.Generic;
using System.Linq;

namespace AI2D.Weapons
{
    public class WeaponGuidedFragMissile : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponGuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponGuidedFragMissile(Core core)
            : base(core, "Guided Frag Missile", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 10;
            FireDelayMilliseconds = 1000;
            Speed = 6;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 600;
            MaxLocks = 2;
            MaxLockOnAngle = 40;
            ExplodesOnImpact = true;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, PointD xyOffset = null)
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
                        var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new PointD(10, 10));
                        _core.Actors.AddNewBullet(this, _owner, pointRight);
                    }
                    else
                    {
                        var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new PointD(10, 10));
                        _core.Actors.AddNewBullet(this, _owner, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var lockedOn in LockedOnObjects)
                    {
                        if (_toggle)
                        {
                            var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new PointD(10, 10));
                            _core.Actors.AddNewLockedBullet(this, _owner, lockedOn, pointRight);
                        }
                        else
                        {
                            var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new PointD(10, 10));
                            _core.Actors.AddNewLockedBullet(this, _owner, lockedOn, pointLeft);
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
