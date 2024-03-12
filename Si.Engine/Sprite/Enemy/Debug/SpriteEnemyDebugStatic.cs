using Si.Engine.Loudout;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebugStatic : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyDebugStatic(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiEnemyClass.Debug;
            SetImage(@$"Graphics\Enemy\Debug\{ShipClass}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            LoadoutEnemyShip loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new LoadoutEnemyShip(ShipClass)
                {
                    Description = "→ Debug Static ←\n"
                       + "Does noting. Sits where you place it..\n"
                       + "When this badboy is spotted, s**t has already hit the proverbial fan.\n",
                    Speed = 0,
                    Boost = 0,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
