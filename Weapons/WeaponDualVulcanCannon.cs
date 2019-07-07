using AI2D.Engine;
using AI2D.GraphicObjects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Weapons
{
    public class WeaponDualVulcanCannon : WeaponBase
    {

        private const string imagePath = @"..\..\Assets\Graphics\Bullet\Vulcan Cannon.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Vulcan Cannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponDualVulcanCannon(Core core)
            : base(core, "Dual Vulcan", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 100;
        }

        public override bool Fire()
        {
            //if (CanFire)
            {
                RoundQuantity--;

                _bulletSound.Play();

                //       ¯\_(ツ)_/¯
                double X = -Math.Cos(_owner.Velocity.Angle.Radians + (90 * (Math.PI / 180))) * 100;
                double Y = Math.Sin(_owner.Velocity.Angle.Radians + (90 * (Math.PI / 180)));


                //       (ノಠ益ಠ)ノ彡┻━┻
                X = _owner.Location.X + 100;
                Y = _owner.Location.Y;

                _core.Actors.Debugs[0].X = X;
                _core.Actors.Debugs[0].Y = Y;

                _core.Actors.CreateBullet(_imagePath, Damage, _owner, new PointD(X, Y));
                //       (ヘ･_･)ヘ┳━┳
                //_core.Actors.CreateBullet(_imagePath, Damage, _owner, new PointD(+10, +10));

                return true;
            }
            return false;

        }
    }
}
