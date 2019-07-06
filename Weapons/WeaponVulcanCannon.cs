using AI2D.Engine;
using AI2D.GraphicObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Weapons
{
    public class WeaponVulcanCannon : WeaponBase
    {
        private const string imagePath = @"..\..\Assets\Graphics\Bullet\Vulcan Cannon.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Vulcan Cannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(Core core)
            : base(core, "Vulcan Cannon", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 100;
        }
    }
}

