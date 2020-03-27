using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyIrlen : BaseEnemy
    {
        public const int ScoreMultiplier = 1;

        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\Irlen\";
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

        public EnemyIrlen(Core core)
            : base(core, BaseEnemy.GetGenericHP(), ScoreMultiplier)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            base.SetHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));

            AddWeapon(new WeaponPhotonTorpedo(_core)
            {
                RoundQuantity = 5,
                FireDelayMilliseconds = 1000,
            });

            AddWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 250
            });

            this.Velocity.Angle.Degrees = AngleTo(_core.Actors.Player);

            initialHitPoints = HitPoints;
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

        int initialHitPoints = 0;
        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        AngleD fallToAngle;
        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(PointD frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioIrlenFormations).
                if (distanceToPlayer < 500 && Utility.ChanceIn(10000) || HitPoints != initialHitPoints)
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + Utility.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (Utility.Random.NextDouble() + 1);
                }
            }

            if (Mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Actors.Player);
                }
                else
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + Utility.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (Utility.Random.NextDouble() + 1);
                }
            }

            if (Mode == AIMode.MovingToFallback)
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
                    Mode = AIMode.MovingToApproach;
                }
            }

            if (Mode == AIMode.MovingToApproach)
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
                    Mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 400 && HasWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponPhotonTorpedo));
                        CurrentWeapon?.Fire();
                    }
                }
                else if (HasWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponVulcanCannon));
                        CurrentWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
