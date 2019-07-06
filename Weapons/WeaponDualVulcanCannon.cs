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


                //var deltaX = x2 - x1;
                //var deltaY = y2 - y1;
                //var rad = Math.atan2(deltaY, deltaX); // In radians

                //Then you can convert it to degrees as easy as:
                //var deg = rad * (180 / Math.PI)


                //var fff = _owner.Velocity.Angle.Vector + new System.Windows.Vector(100, 0);

                //var vOffset = new System.Windows.Vector(Math.Cos(_owner.Velocity.Angle.Degree), Math.Sin(_owner.Velocity.Angle.Degree)) * 0.0;

                //double deltaXa = Math.Cos(_owner.Velocity.Angle.Degree + 90) * 0.02f;
                //double deltaYa = Math.Sin(_owner.Velocity.Angle.Degree + 90) * 100;

                //double deltaX = _owner.Velocity.Angle.X * 100;
                //double deltaY = _owner.Velocity.Angle.Y * 100;


                double X = -Math.Cos(_owner.Velocity.Angle.Radian + (90 * (Math.PI / 180))) * 100;
                double Y = Math.Sin(_owner.Velocity.Angle.Radian + (90 * (Math.PI / 180)));

                X = _owner.Location.X + 100;// + (xyOffset == null ? 0 : xyOffset.X);
                Y = _owner.Location.Y;// + (xyOffset == null ? 0 : xyOffset.Y);


                _core.Actors.Debugs[0].X = X;
                _core.Actors.Debugs[0].Y = Y;


                //_owner.RequiredAngleTo
                //double offsetX = deltaX;
                //double offsetY = deltaY;

                _core.Actors.CreateBullet(_imagePath, Damage, _owner, new PointD(X, Y));
                //_core.Actors.CreateBullet(_imagePath, Damage, _owner, new PointD(+10, +10));

                return true;
            }
            return false;

        }
    }
}
