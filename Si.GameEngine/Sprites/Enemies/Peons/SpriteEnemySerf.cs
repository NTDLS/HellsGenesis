using Si.GameEngine.Engine;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Peons.BasesAndInterfaces;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons;
using Si.Shared;
using Si.Shared.ExtensionMethods;
using Si.Shared.Types.Geometry;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemySerf : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemySerf(EngineCore gameCore)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Serf;
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
                    Description = "→ Serf ←\n"
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

            Velocity.Angle.Degrees = AngleTo360(_gameCore.Player.Sprite);

            _initialHullHealth = HullHealth;
        }

        #region Artificial Intelligence.

        public enum AIMode
        {
            InFormation,
            InFormationTurning,
            Approaching,
            MovingToFallback,
            MovingToApproach,
        }

        private readonly int _initialHullHealth = 0;
        private const double _baseDistanceToKeep = 100;
        private double _distanceToKeep = _baseDistanceToKeep * (SiRandom.Generator.NextDouble() + 1);
        private const double _baseFallbackDistance = 400;
        private double _fallbackDistance;
        private SiAngle _fallToAngle;

        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (ControlledBy == SiControlledBy.Server)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }

            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = SiMath.DistanceTo(this, _gameCore.Player.Sprite);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioSerfFormations).
                if (distanceToPlayer < 500 && SiRandom.PercentChance(10000) || HullHealth != _initialHullHealth)
                {
                    Mode = AIMode.MovingToFallback;
                    _fallToAngle = Velocity.Angle + (180.0 + SiRandom.Between(0, 10));
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.Generator.NextDouble() + 1);
                }
            }

            if (Mode == AIMode.Approaching)
            {
                if (distanceToPlayer > _distanceToKeep)
                {
                    PointAtAndGoto(_gameCore.Player.Sprite);
                }
                else
                {
                    Mode = AIMode.MovingToFallback;
                    _fallToAngle = Velocity.Angle + (180.0 + SiRandom.Between(0, 10));
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.Generator.NextDouble() + 1);
                }
            }

            if (Mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - _fallToAngle;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 1;
                    }
                }

                if (distanceToPlayer > _fallbackDistance)
                {
                    Mode = AIMode.MovingToApproach;
                }
            }

            if (Mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngle(_gameCore.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 10)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 10)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    Mode = AIMode.Approaching;
                    _distanceToKeep = _baseDistanceToKeep * (SiRandom.Generator.NextDouble() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000)
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameCore.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponDualVulcanCannon>();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameCore.Player.Sprite, 2.0);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponVulcanCannon>();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
