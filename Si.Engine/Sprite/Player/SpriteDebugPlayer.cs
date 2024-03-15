using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteDebugPlayer : SpritePlayerBase
    {
        public SpriteDebugPlayer(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiPlayerClass.Debug;

            string imagePath = @$"Sprites\Player\Ships\{ShipClass}.png";
            Initialize(imagePath);

            //Load the loadout from file or create a new one if it does not exist.
            LoadoutPlayerShip loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new LoadoutPlayerShip(ShipClass)
                {
                    Speed = 4.75f,
                    Boost = 1.5f,
                    HullHealth = 10000,
                    ShieldHealth = 10000,
                    Description = "→ The code crusader ←\n"
                        + "Crude in design but equipped with advanced diagnostics and repair systems.\n"
                        + "Nearly indestructible and inconceivably fast. Its mission is to discover\n"
                        + "glitches in the vast cosmic code, ensuring a smooth journey for all that follow...",
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponBlunderbuss), 100000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScattershot), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponBlunderbuss), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponLancer), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100000));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
