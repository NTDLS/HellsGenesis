using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites.Enemies.BaseClasses;
using HG.Utility;
using HG.Utility.ExtensionMethods;
using HG.Weapons;
using System.Drawing;
using System.IO;

namespace HG.Sprites.Enemies.Peons
{
    internal class EnemyIrlen : EnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\Irlen\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyIrlen(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            SetPrimaryWeapon<WeaponPhotonTorpedo>(5);
            AddSecondaryWeapon<WeaponVulcanCannon>(500);

            Velocity.Angle.Degrees = AngleTo(_core.Player.Actor);

            initialHullHealth = HullHealth;
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

        readonly int initialHullHealth = 0;
        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        HgAngle fallToAngle;
        public AIMode Mode = AIMode.InFormation;

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Actor);

            if (Mode == AIMode.InFormation)
            {
                //Since we need to handle the entire "platoon" of formation ships all at once, a good
                //  deal of this AI is handled by the Scenerio engine(s). (see: ScenarioIrlenFormations).
                if (distanceToPlayer < 500 && HgRandom.ChanceIn(10000) || HullHealth != initialHullHealth)
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
                    PointAtAndGoto(_core.Player.Actor);
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
                    if (deltaAngle.Degrees >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 0) //We might as well turn around counter clock-wise
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
                var deltaAngle = DeltaAngle(_core.Player.Actor);

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
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
                }
            }

            if (IsHostile)
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

        #endregion
    }
}
