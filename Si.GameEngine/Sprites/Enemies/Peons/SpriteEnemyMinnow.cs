using Si.GameEngine.Core;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Weapons;
using Si.GameEngine.Utility;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Types.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    /// <summary>
    /// 100% traditional weapons, they enforce their distance and are are moddled to provoke dog fighting. These are fast units.
    /// </summary>
    internal class SpriteEnemyMinnow : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyMinnow(GameEngineCore gameEngine)
            : base(gameEngine, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Minnow;
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
                    Description = "→ Minnow ←\n"
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

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
        }

        private const double baseDistanceToKeep = 200;
        private double distanceToKeep = baseDistanceToKeep * (SiRandom.Generator.NextDouble() + 1);
        private const double baseFallbackDistance = 800;
        private double fallbackDistance;
        private SiAngle fallToAngle;
        private AIMode mode = AIMode.Approaching;
        private int roundsToFireBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (ControlledBy == SiControlledBy.Server)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }

            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = SiMath.DistanceTo(this, _gameEngine.Player.Sprite);

            if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_gameEngine.Player.Sprite);
                }
                else
                {
                    mode = AIMode.Tailing;
                    roundsToFireBeforeTailing = 15;
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_gameEngine.Player.Sprite);

                //Stay on the players tail.
                if (distanceToPlayer > distanceToKeep + 300)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.Approaching;
                }
                else
                {
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }

                //We we get too close, do too much damage or they fire at us enough, they fall back and come in again
                if (distanceToPlayer < distanceToKeep / 2.0
                    || hpRemainingBeforeTailing - HullHealth > 2
                    || roundsToFireBeforeTailing <= 0)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + SiRandom.Between(0, 10));
                    fallbackDistance = baseFallbackDistance * (SiRandom.Generator.NextDouble() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - fallToAngle;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngle(_gameEngine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (SiRandom.Generator.NextDouble() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000)
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameEngine.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponDualVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameEngine.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
