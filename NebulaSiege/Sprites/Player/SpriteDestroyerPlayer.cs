using NebulaSiege.Engine;
using NebulaSiege.Loudouts;
using NebulaSiege.Weapons;
using System.Drawing;
using static NebulaSiege.Loudouts.PlayerShipLoadout;

namespace NebulaSiege.Sprites.Player
{
    internal class SpriteDestroyerPlayer : _SpritePlayerBase
    {
        public SpriteDestroyerPlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Destroyer;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Vicious Annihilator ←\n"
                        + "Lives up to its name as a relentless annihilator,\n"
                        + "unleashing devastating firepower to obliterate foes.",
                    MaxSpeed = 5.0,
                    MaxBoost = 2.5,
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
