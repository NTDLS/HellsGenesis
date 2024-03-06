using Si.Engine.Loudouts;
using Si.Engine.Sprites.Enemies.Peons._Superclass;
using Si.Engine.Sprites.Weapons;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyScav : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyScav(EngineCore engine)
            : base(engine, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Scav;
            SetImage(@$"Graphics\Enemy\Peons\{ShipClass}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Scav ←\n"
                       + "TODO: Add a description\n",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 20,
                    ShieldHealth = 10,
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
