using Si.Engine.Loudout;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyMerc : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyMerc(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiEnemyClass.Merc;
            SetImage(@$"Graphics\Enemy\Peon\{ShipClass}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Merc ←\n"
                       + "TODO: Add a description\n",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 20,
                    ShieldHealth = 10,
                    Bounty = 10
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
