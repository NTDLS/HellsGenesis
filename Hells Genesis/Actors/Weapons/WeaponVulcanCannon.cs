using HG.Actors.Weapons.BaseClasses;
using HG.Engine;

namespace HG.Actors.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        private const string soundPath = @"Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(Core core)
            : base(core, "Vulcan Cannon", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 2;
            FireDelayMilliseconds = 100;
            AngleSlop = 1;
            Speed = 20;
            SpeedSlop = 10;
        }
    }
}
