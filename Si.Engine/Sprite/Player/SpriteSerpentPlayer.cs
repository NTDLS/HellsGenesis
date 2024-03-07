using Si.Engine;
using Si.GameEngine.Loudout;
using Si.GameEngine.Sprite.Player._Superclass;
using Si.GameEngine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.Player
{
    internal class SpriteSerpentPlayer : SpritePlayerBase
    {
        public SpriteSerpentPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Serpent;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Stealthy Serpent ←\n"
                        + "A stealthy long distance fighter, expert in covert operations\n"
                        + "and ambushing unsuspecting adversaries with deadly precision.",
                    Speed = 3.75f,
                    Boost = 2.625f,
                    HullHealth = 100,
                    ShieldHealth = 15000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
