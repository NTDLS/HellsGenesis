using Si.GameEngine.Core;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteStarfighterPlayer : SpritePlayerBase
    {
        public SpriteStarfighterPlayer(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            ShipClass = SiPlayerClass.Starfighter;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Celestial Aviator ←\n"
                        + "A sleek and versatile spacecraft, built for supremacy among the stars.\n"
                        + "It's the first choice for finesse, agility, and unmatched combat prowess\n"
                        + "without all the fuss of a powerful loadout.",
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
