using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Loudouts;
using NebulaSiege.Game.Sprites.Player.BaseClasses;
using NebulaSiege.Game.Weapons;
using System.Drawing;

namespace NebulaSiege.Game.Sprites.Player
{
    internal class SpriteReaverPlayer : SpritePlayerBase
    {
        public SpriteReaverPlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Reaver;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Rogue Reaver ←\n"
                        + "A rogue fighter, known for its hit-and-fade tactics,\n"
                        + "striking and disappearing into the cosmos with warp speed.",
                    MaxSpeed = 5.5,
                    MaxBoost = 3.5,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 8));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
