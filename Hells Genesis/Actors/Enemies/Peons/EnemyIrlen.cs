using HG.Actors.Weapons;
using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies.Peons
{
    internal class EnemyIrlen : EnemyPeonBase
    {
        public const int ScoreMultiplier = 1;
        private const string _assetPath = @"Graphics\Enemy\Irlen\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyIrlen(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            SetHitPoints(HgRandom.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));

            AddSecondaryWeapon(new WeaponPhotonTorpedo(_core)
            {
                RoundQuantity = 5,
                FireDelayMilliseconds = 1000,
            });

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 250
            });

            Velocity.Angle.Degrees = AngleTo(_core.Player.Actor);

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

        readonly int initialHitPoints = 0;
        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        HgAngle<double> fallToAngle;
        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Actor);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioIrlenFormations).
                if (distanceToPlayer < 500 && HgRandom.ChanceIn(10000) || HitPoints != initialHitPoints)
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + HgRandom.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (HgRandom.Random.NextDouble() + 1);
                }
            }

            if (Mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Player.Actor);
                }
                else
                {
                    Mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + HgRandom.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (HgRandom.Random.NextDouble() + 1);
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
                var deltaAngle = DeltaAngle360(_core.Player.Actor);

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
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 400 && HasSecondaryWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponPhotonTorpedo));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (HasSecondaryWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
