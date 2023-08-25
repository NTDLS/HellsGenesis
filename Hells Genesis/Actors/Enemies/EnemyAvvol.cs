using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Weapons;
using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies
{
    /// <summary>
    /// These are a heavy fighting unit, employing various types of weapons and seemingly various stratigies.
    /// They like to stay at a medium range. Some units have seeking-missiles.
    /// </summary>
    internal class EnemyAvvol : EnemyBasicBase
    {
        public const int ScoreMultiplier = 25;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Avvol\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyAvvol(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            SetHitPoints(HgRandom.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));

            Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MaxSpeed - 4, _core.Settings.MaxSpeed - 2); //Upper end of the speed spectrum.

            AddSecondaryWeapon(new WeaponPhotonTorpedo(_core)
            {
                RoundQuantity = 5,
                FireDelayMilliseconds = 1000,
            });

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 100,
                FireDelayMilliseconds = 500
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 100,
                FireDelayMilliseconds = 500
            });

            if (selectedImageIndex == 0 || selectedImageIndex == 2 || selectedImageIndex == 5)
            {
                AddSecondaryWeapon(new WeaponGuidedFragMissile(_core)
                {
                    RoundQuantity = 10,
                    FireDelayMilliseconds = 2000
                });
            }

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
        }

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            MovingToFallback,
            MovingToApproach,
        }

        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        HgAngle<double> fallToAngle;
        AIMode mode = AIMode.Approaching;

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            double distanceToPlayer = HgMath.DistanceTo(this, _core.Player.Actor);

            if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Player.Actor);
                }
                else
                {
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
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (HgRandom.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 500 && HasSecondaryWeaponAndAmmo(typeof(WeaponGuidedFragMissile)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponGuidedFragMissile));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 300 && HasSecondaryWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponPhotonTorpedo));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 200 && HasSecondaryWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 100 && HasSecondaryWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponDualVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
