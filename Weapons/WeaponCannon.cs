using AI2D.Engine;
using AI2D.GraphicObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Weapons
{
    public class WeaponCannon : WeaponBase
    {
        private const string imagePath = @"..\..\Assets\Graphics\Bullet\Laser.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Vulcan Cannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponCannon(Core core)
            : base(core, imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 100;
        }
    }
}

