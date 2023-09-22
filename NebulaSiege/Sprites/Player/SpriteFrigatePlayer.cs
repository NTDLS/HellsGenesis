using NebulaSiege.Engine;
using NebulaSiege.Loudouts;
using NebulaSiege.Weapons;
using System.Drawing;

namespace NebulaSiege.Sprites.Player
{
    internal class SpriteFrigatePlayer : _SpritePlayerBase
    {
        public SpriteFrigatePlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Frigate;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Nimble Interceptor ←\n"
                        + "A nimble interceptor, designed for hit-and-run tactics\n"
                        + "and lightning-fast strikes against enemy forces.",
                    MaxSpeed = 4.5,
                    MaxBoost = 1.5,
                    HullHealth = 500,
                    ShieldHealth = 100,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponScattershot), 10000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
