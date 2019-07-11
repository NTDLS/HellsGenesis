using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyAvvol : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Avvol (1).png",
            "Avvol (2).png",
            "Avvol (3).png",
            "Avvol (4).png",
            "Avvol (5).png",
            "Avvol (6).png",
            "Avvol (7).png",
            "Avvol (8).png"
            #endregion
        };

        public EnemyAvvol(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32,32));
        }

        enum AIMode
        {
            Approaching,
            MovingToFallback,
            MovingToApproach,
        }

        const double baseDistanceToKeep = 100;
        double distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
        const double baseFallbackDistance = 400;
        double fallbackDistance;
        AngleD fallToAngle;
        AIMode mode = AIMode.Approaching;

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
                    if (deltaAngle >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0) //We might as well turn around counter clock-wise
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
                if (distanceToPlayer > 500 && HasWeaponAndAmmo(typeof(WeaponGuidedFragMissile)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponGuidedFragMissile));
                        CurrentWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 300 && HasWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponPhotonTorpedo));
                        CurrentWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 200 && HasWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponVulcanCannon));
                        CurrentWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 100 && HasWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectWeapon(typeof(WeaponDualVulcanCannon));
                        CurrentWeapon?.Fire();
                    }
                }
            }

            /*
            //If the enemy is off the screen, point at the player and come back into view.
            if (X < (0 - (Size.Width + 40)) || Y < (0 - (Size.Height + 40))
                || X >= (_core.Display.VisibleSize.Width + Size.Width) + 40
                || Y >= (_core.Display.VisibleSize.Height + Size.Height) + 40)
            {
                MoveInDirectionOf(_core.Actors.Player);
            }
            */
        }

    }
}
