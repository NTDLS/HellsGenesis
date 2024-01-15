using Si.GameEngine.Core;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDebugPlayer : SpritePlayerBase
    {
        public SpriteDebugPlayer(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            ShipClass = SiPlayerClass.Debug;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    MaxSpeed = 5.0,
                    MaxBoost = 2.0,
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
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 100000));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
