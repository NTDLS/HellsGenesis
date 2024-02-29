using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Weapons;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemySerf : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemySerf(GameEngineCore gameEngine)
            : base(gameEngine, hullHealth, bountyMultiplier)
        {
            ShipClass = SiEnemyClass.Serf;
            SetImage(@$"Graphics\Enemy\Peons\{ShipClass}\Hull.png");

            if (IsDrone)
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
                    Speed = 3.5f,
                    Boost = 1.5f,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            Velocity.Angle.Degrees = AngleTo360(_gameEngine.Player.Sprite);

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
        private const float _baseDistanceToKeep = 100;
        private float _distanceToKeep = _baseDistanceToKeep * (SiRandom.NextFloat() + 1f);
        private const float _baseFallbackDistance = 400;
        private float _fallbackDistance;
        private SiAngle fallToAngleRadians;

        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (IsDrone)
            {
                //If this is a multiplayer drone then we need to skip most of the initilization. This is becuase
                //  the reaminder of the ctor is for adding weapons and initializing AI, none of which we need.
                return;
            }

            base.ApplyIntelligence(epoch, displacementVector);

            float distanceToPlayer = SiVector.DistanceTo(this, _gameEngine.Player.Sprite);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioSerfFormations).
                if (distanceToPlayer < 500 && SiRandom.ChanceIn(1, 50000) || HullHealth != _initialHullHealth)
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.Angle + (180.0f + SiRandom.Between(0, 10));
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (Mode == AIMode.Approaching)
            {
                if (distanceToPlayer > _distanceToKeep)
                {
                    PointAtAndGoto(_gameEngine.Player.Sprite);
                }
                else
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.Angle + new SiAngle(180.0f + SiRandom.Between(0, 10)).Radians;
                    _fallbackDistance = _baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (Mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - fallToAngleRadians;

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
                var deltaAngle = DeltaAngleDegrees(_gameEngine.Player.Sprite);

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
                    _distanceToKeep = _baseDistanceToKeep * (SiRandom.NextFloat() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000)
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameEngine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            FireWeapon<WeaponDualVulcanCannon>();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameEngine.Player.Sprite, 2.0f);
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
