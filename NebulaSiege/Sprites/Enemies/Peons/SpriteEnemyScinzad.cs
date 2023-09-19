using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Utility;
using NebulaSiege.Utility.ExtensionMethods;
using NebulaSiege.Weapons;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Sprites.Enemies.Peons
{
    /// <summary>
    /// 100% traditional weapons, they enforce their distance and are are moddled to provoke dog fighting. These are fast units.
    /// </summary>
    internal class SpriteEnemyScinzad : _SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\Scinzad\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;


        public SpriteEnemyScinzad(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            Velocity.MaxSpeed = HgRandom.Generator.Next(_core.Settings.MaxEnemySpeed - 2, _core.Settings.MaxEnemySpeed); //Upper end of the speed spectrum

            SetPrimaryWeapon<WeaponVulcanCannon>(1000);
            AddSecondaryWeapon<WeaponDualVulcanCannon>(500);
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
        private double distanceToKeep = baseDistanceToKeep * (HgRandom.Generator.NextDouble() + 1);
        private const double baseFallbackDistance = 800;
        private double fallbackDistance;
        private NsAngle fallToAngle;
        private AIMode mode = AIMode.Approaching;
        private int bulletsRemainingBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Sprite);

            if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_core.Player.Sprite);
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
                PointAtAndGoto(_core.Player.Sprite);

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
                    fallToAngle = Velocity.Angle + (180.0 + HgRandom.Between(0, 10));
                    fallbackDistance = baseFallbackDistance * (HgRandom.Generator.NextDouble() + 1);
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
                var deltaAngle = DeltaAngle(_core.Player.Sprite);

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
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Generator.NextDouble() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 800)
                {
                    if (distanceToPlayer > 400 && HasSelectedSecondaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 8.0);
                        if (isPointingAtPlayer)
                        {
                            SelectedSecondaryWeapon?.Fire();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasSelectedPrimaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 15.0);
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
