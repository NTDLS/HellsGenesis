using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Loudouts;
using NebulaSiege.Game.Sprites.Player.BaseClasses;
using NebulaSiege.Game.Weapons;
using System.Drawing;

namespace NebulaSiege.Game.Sprites.Player
{
    internal class SpriteDreadnaughtPlayer : SpritePlayerBase
    {
        public SpriteDreadnaughtPlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Dreadnaught;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Titanic Dreadnought ←\n"
                        + "Titanic force of destruction, capable of withstanding\n"
                        + "immense firepower while dishing out colossal damage.",
                    MaxSpeed = 4.0,
                    MaxBoost = 2.0,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 500)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 10));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 6));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 4));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
