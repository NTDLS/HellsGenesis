using HG.Actors.Weapons;
using System.Collections.Generic;
using System.Linq;
using static HG.Engine.Constants;
using static HG.Loudouts.ShipLoadout;

namespace HG.Loudouts
{
    internal class PrefabPlayerLoadouts
    {
        public List<ShipLoadout> Collection { get; private set; } = new();

        public ShipLoadout GetByName(string name)
        {
            return Collection.Where(o => o.Name == name).First();
        }

        public ShipLoadout GetDefault()
        {
            return Collection.First();
        }

        public void CreateDefaults()
        {
#if DEBUG
            //Debug
            {
                var loadout = new ShipLoadout(PlayerClass.Debug)
                {
                    Speed = 3.0,
                    Boost = 4.0,
                    Hull = 10000,
                    Sheilds = 10000,
                    Description = "This ship might look crude, but its faster than the bullets and missiles.\r\n It feels like a glitttch in thƏ m@tR|x...",
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponHotPepper), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100000));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 100000));

                Collection.Add(loadout);
            }
#endif
            //Frigate
            {
                var loadout = new ShipLoadout(PlayerClass.Frigate)
                {
                    Description = "The Frigate...",
                    Speed = 3.0,
                    Boost = 1.5,
                    Hull = 500,
                    Sheilds = 100,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponHotPepper), 10000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                //loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 500)); //I dont have a second non guided missile.

                Collection.Add(loadout);
            }

            //Cruiser
            {
                var loadout = new ShipLoadout(PlayerClass.Cruiser)
                {
                    Description = "The Cruiser...",
                    Speed = 2.2,
                    Boost = 1.0,
                    Hull = 2500,
                    Sheilds = 3000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                //loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 500 )); //I dont have a second non guided missile.

                Collection.Add(loadout);
            }

            //Destroyer
            {
                var loadout = new ShipLoadout(PlayerClass.Destroyer)
                {
                    Description = "The Destroyer...",
                    Speed = 5.0,
                    Boost = 2.5,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 16));

                Collection.Add(loadout);
            }

            //Dreadnaught
            {
                var loadout = new ShipLoadout(PlayerClass.Dreadnaught)
                {
                    Description = "The Dreadnaught...",
                    Speed = 4.0,
                    Boost = 2.0,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 500)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 16));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponGuidedFragMissile), 10));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPrecisionGuidedFragMissile), 6));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponScramsMissile), 4));

                Collection.Add(loadout);
            }

            //Reaver
            {
                var loadout = new ShipLoadout(PlayerClass.Reaver)
                {
                    Description = "The Reaver...",
                    Speed = 6.0,
                    Boost = 3.5,
                    Hull = 500,
                    Sheilds = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 8));

                Collection.Add(loadout);
            }

            //Serpent
            {
                var loadout = new ShipLoadout(PlayerClass.Serpent)
                {
                    Description = "The Serpent...",
                    Speed = 5.0,
                    Boost = 3.5,
                    Hull = 100,
                    Sheilds = 15000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));

                Collection.Add(loadout);
            }
        }
    }
}
