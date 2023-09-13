using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Ordinary;
using HG.Actors.Weapons;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;
using HG.Utility.ExtensionMethods;

namespace HG.Actors.Enemies.Bosses
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    internal class EnemySpectre : EnemyBossBase
    {
        public const int bountyMultiplier = 15;

        private readonly ActorAttachment _leftGun;
        private readonly ActorAttachment _rightGun;
        private readonly ActorAttachment _rightThrust;
        private readonly ActorAttachment _leftThrust;

        private readonly double _initialMaxpeed;

        readonly string _imagesPath = @"Graphics\Enemy\Spectre\";

        public EnemySpectre(Core core)
            : base(core, GetGenericHP(core), bountyMultiplier)
        {
            _leftGun = Attach(_imagesPath + "Gun.Left.png", true, 3);
            _rightGun = Attach(_imagesPath + "Gun.Right.png", true, 3);
            _leftThrust = Attach(_imagesPath + "Jet.png", true, 3);
            _rightThrust = Attach(_imagesPath + "Jet.png", true, 3);

            SetHullHealth(HgRandom.Random.Next(Settings.MinEnemyHealth, Settings.MaxEnemyHealth));

            _initialMaxpeed = HgRandom.Random.Next(Settings.MaxSpeed - 2, Settings.MaxSpeed); //Upper end of the speed spectrum

            Velocity.MaxSpeed = _initialMaxpeed;

            SetImage(_imagesPath + "Hull.png");

            SetPrimaryWeapon<WeaponVulcanCannon>(1000);
            AddSecondaryWeapon<WeaponDualVulcanCannon>(500);
        }

        public override void VelocityChanged()
        {
            if (_leftThrust != null && _rightThrust != null)
            {
                bool visibleThrust = Velocity.ThrottlePercentage > 0;

                if (_leftThrust.IsDead == false)
                {
                    _leftThrust.Visable = visibleThrust;
                }
                if (_rightThrust.IsDead == false)
                {
                    _rightThrust.Visable = visibleThrust;
                }
            }
        }

        public override void PositionChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun.IsDead == false)
                {
                    var pointLeft = HgMath.AngleFromPointAtDistance(Velocity.Angle - 90, new HgPoint(25, 25));
                    _leftGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _leftGun.X = X + pointLeft.X;
                    _leftGun.Y = Y + pointLeft.Y;
                }

                if (_rightGun.IsDead == false)
                {
                    var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 90, new HgPoint(25, 25));
                    _rightGun.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _rightGun.X = X + pointRight.X;
                    _rightGun.Y = Y + pointRight.Y;
                }

                if (_leftThrust.IsDead == false)
                {
                    var pointLeft = HgMath.AngleFromPointAtDistance(Velocity.Angle - 135, new HgPoint(35, 35));
                    _leftThrust.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _leftThrust.X = X + pointLeft.X;
                    _leftThrust.Y = Y + pointLeft.Y;
                }

                if (_rightThrust.IsDead == false)
                {
                    var pointRight = HgMath.AngleFromPointAtDistance(Velocity.Angle + 135, new HgPoint(35, 35));
                    _rightThrust.Velocity.Angle.Degrees = Velocity.Angle.Degrees;
                    _rightThrust.X = X + pointRight.X;
                    _rightThrust.Y = Y + pointRight.Y;
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
        private double distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
        private const double baseFallbackDistance = 800;
        private double fallbackDistance;
        private HgAngle fallToAngle;
        private AIMode mode = AIMode.Approaching;
        private int bulletsRemainingBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Actor);

            //We have no engines. :(
            if (_leftThrust.IsDead && _rightThrust.IsDead)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            int thrustHandicap = (_leftThrust.IsDead ? 0 : 1) + (_rightThrust.IsDead ? 0 : 1);
            if (thrustHandicap == 1 && Velocity.MaxSpeed > _initialMaxpeed / 2)
            {
                Velocity.MaxSpeed -= 0.5;
            }
            if (thrustHandicap == 0 && Velocity.MaxSpeed > 1)
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
                var deltaAngle = DeltaAngle(_core.Player.Actor);

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
                    PointAtAndGoto(_core.Player.Actor);
                }
                else
                {
                    mode = AIMode.Tailing;
                    bulletsRemainingBeforeTailing = TotalAvailableSecondaryWeaponRounds();
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_core.Player.Actor);

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
                    || bulletsRemainingBeforeTailing - TotalAvailableSecondaryWeaponRounds() > 15)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + HgRandom.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (HgRandom.Random.NextDouble() + 1);
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
                var deltaAngle = DeltaAngle(_core.Player.Actor);

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
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 700 && (_rightGun.IsDead == false || _leftGun.IsDead == false))
                {
                    if (distanceToPlayer < 800)
                    {
                        if (distanceToPlayer > 400 && HasSelectedSecondaryWeaponAndAmmo())
                        {
                            bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                            if (isPointingAtPlayer)
                            {
                                SelectedSecondaryWeapon?.Fire();
                            }
                        }
                        else if (distanceToPlayer > 0 && HasSelectedPrimaryWeaponAndAmmo())
                        {
                            bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 15.0);
                            if (isPointingAtPlayer)
                            {
                                PrimaryWeapon?.Fire();
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
