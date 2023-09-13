using HG.Actors.BaseClasses;
using HG.Actors.Weapons.BaseClasses;
using HG.Actors.Weapons.Bullets;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types;

namespace HG.Actors.Weapons
{
    internal class WeaponScattershot : WeaponBase
    {
        static new string Name { get; } = "Scattershot";
        private const string soundPath = @"Sounds\Weapons\WeaponVulcanCannon.wav";
        private const float soundVolumne = 0.2f;

        public WeaponScattershot(Core core, ActorShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponScattershot(Core core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            RoundQuantity = 500;
            Damage = 1;
            FireDelayMilliseconds = 25;
            AngleVariance = 4;
            Speed = 15;
            SpeedVariance = 10;
        }

        public override BulletBase CreateBullet(ActorBase lockedTarget, HgPoint<double> xyOffset = null)
        {
            return new BulletScattershot(_core, this, _owner, lockedTarget, xyOffset);
        }
    }
}
