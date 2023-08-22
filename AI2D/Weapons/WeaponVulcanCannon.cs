using AI2D.Engine;

namespace AI2D.Weapons
{
    internal class WeaponVulcanCannon : WeaponBase
    {
        private const string soundPath = @"..\..\..\Assets\Sounds\Weapons\WeaponVulcanCannon.wav";
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
