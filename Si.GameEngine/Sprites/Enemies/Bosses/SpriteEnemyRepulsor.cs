using Si.GameEngine.Engine;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Enemies.Bosses.BasesAndInterfaces;
using Si.GameEngine.Utility;
using Si.GameEngine.Weapons;
using Si.Shared;
using Si.Shared.ExtensionMethods;
using Si.Shared.Types.Geometry;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Enemies.Bosses
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    internal class SpriteEnemyRepulsor : SpriteEnemyBossBase
    {
        public const int hullHealth = 100;
        public const int bountyMultiplier = 15;

        private readonly SpriteAttachment _leftGun;
        private readonly SpriteAttachment _rightGun;
        private readonly SpriteAttachment _thrust;

        private readonly double _initialMaxpeed;

        readonly string _assetPath = @"Graphics\Enemy\Bosses\Repulsor\";

        public SpriteEnemyRepulsor(EngineCore gameCore)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            _leftGun = Attach(_assetPath + "Gun.Left.png", true, 3);
            _rightGun = Attach(_assetPath + "Gun.Right.png", true, 3);
            _thrust = Attach(_assetPath + "Jet.png", true, 3);

            SetImage(_assetPath + "Hull.png");

            ShipClass = SiEnemyClass.Repulsor;

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Repulsor ←\n"
                       + "TODO: Add a description\n",
                    MaxSpeed = 3.5,
                    MaxBoost = 1.5,
                    HullHealth = 2500,
                    ShieldHealth = 3000,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            _initialMaxpeed = Velocity.MaxSpeed;
        }

        public override void VelocityChanged()
        {
            if (_thrust != null)
            {
                bool visibleThrust = Velocity.ThrottlePercentage > 0;

                if (_thrust.IsDead == false)
                {
                    _thrust.Visable = visibleThrust;
                }
            }
        }

        public override void PositionChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun?.IsDead == false)
                {
                    var pointLeft = SiMath.PointFromAngleAtDistance360(Velocity.Angle - 90, new SiPoint(25, 25));
                    _leftGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _leftGun.LocalX = LocalX + pointLeft.X;
                    _leftGun.LocalY = LocalY + pointLeft.Y;
                }

                if (_rightGun?.IsDead == false)
                {
                    var pointRight = SiMath.PointFromAngleAtDistance360(Velocity.Angle + 90, new SiPoint(25, 25));
                    _rightGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _rightGun.LocalX = LocalX + pointRight.X;
                    _rightGun.LocalY = LocalY + pointRight.Y;
                }

                if (_thrust?.IsDead == false)
                {
                    var pointRight = SiMath.PointFromAngleAtDistance360(Velocity.Angle + 180, new SiPoint(35, 35));
                    _thrust.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _thrust.LocalX = LocalX + pointRight.X;
                    _thrust.LocalY = LocalY + pointRight.Y;
                }
            }
        }

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
            LameDuck
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
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = SiMath.DistanceTo(this, _gameCore.Player.Sprite);

            //We have no engines. :(
            if (_thrust?.IsDead == true)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            if (_thrust?.IsDead == true)
            {
                Velocity.MaxSpeed -= 0.5;
                if (Velocity.MaxSpeed < 1)
                {
                    Velocity.MaxSpeed = 1;
                }
            }

            if (mode == AIMode.LameDuck)
            {
                if (distanceToPlayer > 2500)
                {
                    Explode();
                }

                //Keep pointing at the player.
                var deltaAngle = DeltaAngle(_gameCore.Player.Sprite);

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

                //Try to stay close.
                if (distanceToPlayer > 300)
                {
                    Velocity.ThrottlePercentage += 0.05;
                    if (Velocity.ThrottlePercentage > 1)
                    {
                        Velocity.ThrottlePercentage = 1;
                    }
                }
                else
                {
                    //Slow to a stop when close.
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }
            }
            else if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_gameCore.Player.Sprite);
                }
                else
                {
                    mode = AIMode.Tailing;
                    roundsToFireBeforeTailing = 25;
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_gameCore.Player.Sprite);

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
                var deltaAngle = DeltaAngle(_gameCore.Player.Sprite);

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
                if (distanceToPlayer < 1000 && (_rightGun?.IsDead == false || _leftGun?.IsDead == false))
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_gameCore.Player.Sprite, 2.0);
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
                        bool isPointingAtPlayer = IsPointingAt(_gameCore.Player.Sprite, 2.0);
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