﻿using Si.GameEngine.Engine;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Peons.BasesAndInterfaces;
using Si.GameEngine.Weapons;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.IO;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyUves : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyUves(EngineCore gameCore)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Uves;
            SetImage(@$"Graphics\Enemy\{ShipClass}\Hull.png", new Size(32, 32));

            if (ControlledBy == SiControlledBy.Server)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Uves ←\n"
                       + "TODO: Add a description\n",
                    MaxSpeed = 3.5,
                    MaxBoost = 1.5,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (ControlledBy == SiControlledBy.Server)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }
        }
    }
}
