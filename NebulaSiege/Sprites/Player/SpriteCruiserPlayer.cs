using NebulaSiege.Engine;
using NebulaSiege.Loudouts;
using NebulaSiege.Weapons;
using System.Drawing;
using static NebulaSiege.Loudouts.ShipLoadout;

namespace NebulaSiege.Sprites.Player
{
    internal class SpriteCruiserPlayer : _SpritePlayerBase
    {
        public SpriteCruiserPlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Cruiser;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            ShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new ShipLoadout(ShipClass)
                {
                    Description = "→ Heavy Assault Cruiser ←\n"
                       + "A formidable heavy assault vessel, bristling with weaponry\n"
                       + "and to take on any adversary in head-to-head combat.",
                    MaxSpeed = 3.5,
                    MaxBoost = 1.5,
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
