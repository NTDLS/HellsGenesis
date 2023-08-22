using AI2D.Actors.Objects.Weapons.Bullets;
using AI2D.Engine;
using AI2D.Types;

namespace AI2D.Actors.Objects.Weapons
{
    internal class WeaponFragMissile : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponFragMissile(Core core)
            : base(core, "Frag Missile", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 10;
            FireDelayMilliseconds = 250;
            Speed = 10;

            CanLockOn = false;
            ExplodesOnImpact = true;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, Point<double> xyOffset = null)
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
                        var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new Point<double>(10, 10));
                        _core.Actors.Bullets.Create(this, _owner, pointRight);
                    }
                    else
                    {
                        var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new Point<double>(10, 10));
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
