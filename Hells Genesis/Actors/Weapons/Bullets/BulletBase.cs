using HG.Actors.Enemies;
using HG.Engine;
using HG.Types;
using System;
using System.Drawing;

namespace HG.Actors.Weapons.Bullets
{
    internal class BulletBase : ActorBase
    {
        public HgFiredFromType FiredFromType { get; set; }
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
             ActorBase lockedTarget = null, HgPoint<double> xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            LockedTarget = lockedTarget;
            Velocity.ThrottlePercentage = 100;

            RadarDotSize = new HgPoint<int>(2, 2);
            RadarDotColor = Color.FromArgb(255, 0, 0);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;

            if (weapon.AngleSlop != null)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * (double)weapon.AngleSlop);
            }

            if (firedFrom is EnemyBase)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * 2);
            }

            HgVelocity<double> initialVelocity = new HgVelocity<double>()
            {
                Angle = new HgAngle<double>(headingDegrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 100
            };

            if (Weapon.SpeedSlop != null)
            {
                initialVelocity.MaxSpeed += (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * (double)weapon.AngleSlop);
            }

            var initialLocation = firedFrom.Location;
            initialLocation.X = initialLocation.X + (xyOffset == null ? 0 : xyOffset.X);
            initialLocation.Y = initialLocation.Y + (xyOffset == null ? 0 : xyOffset.Y);
            Location = initialLocation;

            if (firedFrom is EnemyBase)
            {
                FiredFromType = HgFiredFromType.Enemy;
            }
            else if (firedFrom is ActorPlayer)
            {
                FiredFromType = HgFiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(HgPoint<double> displacementVector, ActorBase testHit)
        {
            if (AgeInMilliseconds > MaxAgeInMilliseconds)
            {
                Explode();
                return;
            }

            if (FiredFromType == HgFiredFromType.Enemy && !(testHit is EnemyBase))
            {
                if (Intersects(_core.Player.Actor))
                {
                    //We don't auto delete the player because there is only one instance, the engine always assumes its valid.
                    testHit.Hit(this, true, false);
                    if (this is BulletPulseMeson == false)
                    {
                        Explode();
                    }
                }
            }
            else if (FiredFromType == HgFiredFromType.Player && !(testHit is ActorPlayer))
            {
                if (Intersects(testHit))
                {
                    testHit.Hit(this);

                    if (this is BulletPulseMeson == false)
                    {
                        Explode();
                    }
                }
            }
        }

        public new void ApplyMotion(HgPoint<double> displacementVector)
        {
            if (X < -_core.Settings.BulletSceneDistanceLimit
                || X >= _core.Display.TotalCanvasSize.Width + _core.Settings.BulletSceneDistanceLimit
                || Y < -_core.Settings.BulletSceneDistanceLimit
                || Y >= _core.Display.TotalCanvasSize.Height + _core.Settings.BulletSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
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
