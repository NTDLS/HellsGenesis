using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    /// <summary>
    /// 100% traditional weapons, they enforce their distance and are are moddled to provoke dog fighting. These are fast units.
    /// </summary>
    public class EnemyScinzad : BaseEnemy
    {
        public const int ScoreMultiplier = 15;

        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\Scinzad\";
        private readonly string[] _imagePaths = {
            #region images.
            "1.png",
            "2.png",
            "3.png",
            "4.png",
            "5.png",
            "6.png"
            #endregion
        };

        public EnemyScinzad(Core core)
            : base(core, BaseEnemy.GetGenericHP(), ScoreMultiplier)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();
            HitPoints = Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);
            Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MaxSpeed - 2, Constants.Limits.MaxSpeed); //Upper end of the speed spectrum

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));

            AddWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 1000,
                FireDelayMilliseconds = 250
            });

            AddWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 500
            });

            SelectWeapon(typeof(WeaponVulcanCannon));
        }

        #region Artificial Intelligence.

        enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
        }

        const double baseDistanceToKeep = 200;
        double distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
        const double baseFallbackDistance = 800;
        double fallbackDistance;
        AngleD fallToAngle;
        AIMode mode = AIMode.Approaching;
        int bulletsRemainingBeforeTailing = 0;
        int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(PointD frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);

            if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Actors.Player);
                }
                else
                {
                    mode = AIMode.Tailing;
                    bulletsRemainingBeforeTailing = this.TotalAvailableRounds();
                    hpRemainingBeforeTailing = this.HitPoints;
                }
            }

            if (mode == AIMode.Tailing)
            {
                MoveInDirectionOf(_core.Actors.Player);

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

                if (distanceToPlayer < (distanceToKeep / 2.0)
                    || (hpRemainingBeforeTailing - this.HitPoints) > 2
                    || (bulletsRemainingBeforeTailing - this.TotalAvailableRounds()) > 15)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + Utility.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (Utility.Random.NextDouble() + 1);
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
                var deltaAngle = DeltaAngle(_core.Actors.Player);

                if (deltaAngle > 10)
                {
                    if (deltaAngle >= 180.0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 200 && HasWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponVulcanCannon));
                        CurrentWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 0 && HasWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponDualVulcanCannon));
                        CurrentWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
