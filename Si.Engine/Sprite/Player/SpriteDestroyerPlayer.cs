using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteDestroyerPlayer : SpritePlayerBase
    {
        public SpriteDestroyerPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Destroyer;

            string imagePath = @$"Sprites\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            LoadoutPlayerShip loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new LoadoutPlayerShip(ShipClass)
                {
                    Description = "→ Vicious Annihilator ←\n"
                        + "Lives up to its name as a relentless annihilator,\n"
                        + "unleashing devastating firepower to obliterate foes.",
                    Speed = 3.75f,
                    Boost = 1.875f,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 42));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
