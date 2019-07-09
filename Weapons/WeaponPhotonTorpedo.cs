using AI2D.Engine;
using AI2D.GraphicObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Weapons
{
    public class WeaponPhotonTorpedo : WeaponBase
    {
        private const string imagePath = @"..\..\Assets\Graphics\Weapon\Photon Torpedo.png";
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Photon Torpedo.wav";
        private const float soundVolumne = 0.4f;

        public WeaponPhotonTorpedo(Core core)
            : base(core, "Photon Torpedo", imagePath, soundPath, soundVolumne)
        {
            RoundQuantity = 10;
            Damage = 5;
            FireDelayMilliseconds = 500;
        }
    }
}

