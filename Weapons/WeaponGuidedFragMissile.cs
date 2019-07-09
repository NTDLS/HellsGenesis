using AI2D.Engine;
using AI2D.GraphicObjects;
using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AI2D.Weapons
{
    public class WeaponGuidedFragMissile : WeaponBase
    {
        private const string imagePath = @"..\..\Assets\Graphics\Weapon\Missiles\Missile 05 (3).png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Guided Frag Missile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponGuidedFragMissile(Core core)
            : base(core, "Guided Frag Missile", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 25;
            FireDelayMilliseconds = 1000;
            Speed = 6;

            CanLockOn = true;
            MinLockDistance = 50;
            MaxLockDistance = 600;
            MaxLocks = 4;
            MaxLockOnAngle = 40;
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _bulletSound.Play();
                RoundQuantity--;

                //Animated bullet
                //_core.Actors.CreateAnimatedBullet(_imagePath, Damage, _owner, new Size(32, 32), 10, pointRight);

                List<BaseEnemy> lockedTargets = (from o in _core.Actors.Enemies where o.IsLockedOn == true select o).ToList();

                if (lockedTargets == null || lockedTargets.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new PointD(10, 10));
                        _core.Actors.CreateBullet(_imagePath, this, _owner, pointRight);
                    }
                    else
                    {
                        var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new PointD(10, 10));
                        _core.Actors.CreateBullet(_imagePath, this, _owner, pointLeft);
                    }
                }
                else
                {
                    foreach (var enemy in lockedTargets)
                    {
                        //if (_toggle)
                        //{
                            //var pointRight = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new PointD(10, 10));
                            _core.Actors.CreateLockedBullet(_imagePath, this, _owner, enemy);
                        //}
                        //else
                        //{
                            //var pointLeft = Utility.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new PointD(10, 10));
                            //_core.Actors.CreateLockedBullet(_imagePath, this, _owner, enemy, pointLeft);
                        //}
                    }
                }

                _toggle = !_toggle;

                return true;
            }
            return false;

        }
    }
}
