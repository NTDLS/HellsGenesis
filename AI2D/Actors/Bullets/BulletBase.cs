using AI2D.Actors.Enemies;
using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Drawing;

namespace AI2D.Actors.Bullets
{
    public class BulletBase : ActorBase
    {
        public FiredFromType FiredFromType { get; set; }
        public WeaponBase Weapon { get; private set; }
        public ActorBase LockedTarget { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MaxAgeInMilliseconds { get; set; } = 4000;
        public double AgeInMilliseconds
        {
            get
            {
                return (DateTime.UtcNow - CreatedDate).TotalMilliseconds;
            }
        }

        public BulletBase(Core core, WeaponBase weapon, ActorBase firedFrom, string imagePath,
             ActorBase lockedTarget = null, Point<double> xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            LockedTarget = lockedTarget;
            Velocity.ThrottlePercentage = 100;

            this.RadarDotSize = new Point<int>(2, 2);
            this.RadarDotColor = Color.FromArgb(255, 0, 0);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;

            if (weapon.AngleSlop != null)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (Utility.FlipCoin() ? 1 : -1) * (Utility.Random.NextDouble() * (double)weapon.AngleSlop);
            }

            if (firedFrom is EnemyBase)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (Utility.FlipCoin() ? 1 : -1) * (Utility.Random.NextDouble() * 2);
            }

            Velocity<double> initialVelocity = new Velocity<double>()
            {
                Angle = new Angle<double>(headingDegrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 100
            };

            if (Weapon.SpeedSlop != null)
            {
                initialVelocity.MaxSpeed += (Utility.FlipCoin() ? 1 : -1) * (Utility.Random.NextDouble() * (double)weapon.AngleSlop);
            }

            var initialLocation = firedFrom.Location;
            initialLocation.X = initialLocation.X + (xyOffset == null ? 0 : xyOffset.X);
            initialLocation.Y = initialLocation.Y + (xyOffset == null ? 0 : xyOffset.Y);
            Location = initialLocation;

            if (firedFrom is EnemyBase)
            {
                FiredFromType = FiredFromType.Enemy;
            }
            else if (firedFrom is ActorPlayer)
            {
                FiredFromType = FiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(Point<double> frameAppliedOffset, ActorBase testHit)
        {
            if (AgeInMilliseconds > MaxAgeInMilliseconds)
            {
                Explode();
                return;
            }

            if (FiredFromType == FiredFromType.Enemy && !(testHit is EnemyBase))
            {
                if (Intersects(_core.Actors.Player))
                {
                    //We don't auto delete the player because there is only one instance, the engine always assumes its valid.
                    testHit.Hit(this, true, false);
                    if ((this is BulletPulseMeson) == false)
                    {
                        Explode();
                    }
                }
            }
            else if (FiredFromType == FiredFromType.Player && !(testHit is ActorPlayer))
            {
                if (Intersects(testHit))
                {
                    testHit.Hit(this);

                    if ((this is BulletPulseMeson) == false)
                    {
                        Explode();
                    }
                }
            }
        }

        public virtual void ApplyMotion(Point<double> frameAppliedOffset)
        {
            if (X < -Constants.Limits.BulletSceneDistanceLimit
                || X >= _core.Display.TotalCanvasSize.Width + Constants.Limits.BulletSceneDistanceLimit
                || Y < -Constants.Limits.BulletSceneDistanceLimit
                || Y >= _core.Display.TotalCanvasSize.Height + Constants.Limits.BulletSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            X += (Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.X;
            Y += (Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.Y;
        }

        public virtual void Explode()
        {
            if (Weapon != null && Weapon.ExplodesOnImpact)
            {
                HitExplosion();
            }
            QueueForDelete(); ;
        }
    }
}
