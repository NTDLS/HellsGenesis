using Si.GameEngine.AI.Logistics;
using Si.GameEngine.Core;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Weapons;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System;
using System.Drawing;
using System.Linq;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyPhoenix : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyPhoenix(Engine gameCore)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Phoenix;
            SetImage(@$"Graphics\Enemy\Peons\{ShipClass}\Hull.png", new Size(32, 32));

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
                    Description = "→ Phoenix ←\n"
                       + "TODO: Add a description\n",
                    MaxSpeed = 3.5,
                    MaxBoost = 1.5,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            //AddAIController(new HostileEngagement(_gameCore, this, _gameCore.Player.Sprite));
            AddAIController(new Taunt(_gameCore, this, _gameCore.Player.Sprite));
            //AddAIController(new Meander(_gameCore, this, _gameCore.Player.Sprite));

            //if (SiRandom.FlipCoin())
            //{
            SetCurrentAIController(AIControllers[typeof(Taunt)]);
            //}
            //else
            //{
            //    SetDefaultAIController(AIControllers[typeof(Meander)]);
            //}

            _behaviorChangeThresholdMiliseconds = SiRandom.Between(2000, 10000);

            Velocity.ThrottlePercentage = 0;
            Velocity.BoostPercentage = 0;
        }

        #region Artificial Intelligence.

        private DateTime _lastBehaviorChangeTime = DateTime.Now;
        private double _behaviorChangeThresholdMiliseconds = 0;

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            return;

            if (ControlledBy == SiControlledBy.Server)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }


            //double distanceToPlayer = SiMath.DistanceTo(this, _gameCore.Player.Sprite);

            base.ApplyIntelligence(displacementVector);

            if ((DateTime.Now - _lastBehaviorChangeTime).TotalMilliseconds > _behaviorChangeThresholdMiliseconds)
            {
                _behaviorChangeThresholdMiliseconds = SiRandom.Between(2000, 10000);

                /*
                if (SiRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(HostileEngagement)]);
                }
                if (SiRandom.ChanceIn(2))
                {
                */
                SetCurrentAIController(AIControllers[typeof(Taunt)]);
                /*
                }
                else if (SiRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(Meander)]);
                }
                */
            }

            if (IsHostile)
            {
                var playersIAmPointingAt = GetPointingAtOf(_gameCore.Sprites.AllVisiblePlayers, 2.0);
                if (playersIAmPointingAt.Any())
                {
                    var closestDistance = ClosestDistanceOf(playersIAmPointingAt);

                    if (closestDistance < 1000)
                    {
                        if (closestDistance > 500 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                        {
                            FireWeapon<WeaponVulcanCannon>();
                        }
                        else if (closestDistance > 0 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                        {
                            FireWeapon<WeaponDualVulcanCannon>();
                        }
                    }
                }
            }

            CurrentAIController?.ApplyIntelligence(displacementVector);
        }

        #endregion
    }
}
