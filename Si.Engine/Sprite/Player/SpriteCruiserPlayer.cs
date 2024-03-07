using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteCruiserPlayer : SpritePlayerBase
    {
        public SpriteCruiserPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Cruiser;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Heavy Assault Cruiser ←\n"
                       + "A formidable heavy assault vessel, bristling with weaponry\n"
                       + "and to take on any adversary in head-to-head combat.",
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 2500,
                    ShieldHealth = 3000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));


                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
