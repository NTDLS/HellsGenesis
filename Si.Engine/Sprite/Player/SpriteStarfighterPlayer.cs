using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteStarfighterPlayer : SpritePlayerBase
    {
        public SpriteStarfighterPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Starfighter;

            string imagePath = @$"Sprites\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            LoadoutPlayerShip loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new LoadoutPlayerShip(ShipClass)
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
