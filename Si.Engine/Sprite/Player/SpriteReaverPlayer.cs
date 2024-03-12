using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteReaverPlayer : SpritePlayerBase
    {
        public SpriteReaverPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Reaver;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            LoadoutPlayerShip loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new LoadoutPlayerShip(ShipClass)
                {
                    Description = "→ Rogue Reaver ←\n"
                        + "A rogue fighter, known for its hit-and-fade tactics,\n"
                        + "striking and disappearing into the cosmos with warp speed.",
                    Speed = 4.125f,
                    Boost = 2.625f,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponLancer), 100));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 8));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
