using AI2D.Engine;

namespace AI2D.Weapons
{
    public class WeaponVulcanCannon : WeaponBase
    {
        private const string soundPath = @"..\..\Assets\Sounds\Weapons\Vulcan Cannon.wav";
        private const float soundVolumne = 0.4f;

        public WeaponVulcanCannon(Core core)
            : base(core, "Vulcan Cannon", soundPath, soundVolumne)
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 100;
        }
    }
}
