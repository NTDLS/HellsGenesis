using HG.Actors.BaseClasses;
using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Ordinary;
using HG.Actors.Weapons.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Types.Geometry;
using HG.Utility;
using System;

namespace HG.Actors.Weapons.Bullets.BaseClasses
{
    internal class BulletBase : ActorBase
    {
        public HgFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public ActorBase LockedTarget { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        public BulletBase(Core core, WeaponBase weapon, ActorBase firedFrom, string imagePath,
             ActorBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            LockedTarget = lockedTarget;
            Velocity.ThrottlePercentage = 1.0;

            RadarDotSize = new HgPoint(2, 2);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;

            if (weapon.AngleVariance != null)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * (double)weapon.AngleVariance);
            }

            if (firedFrom is EnemyBase)
            {
                headingDegrees = firedFrom.Velocity.Angle.Degrees + (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * 2);
            }

            var initialVelocity = new HgVelocity()
            {
                Angle = new HgAngle(headingDegrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 1.0
            };

            if (Weapon.SpeedVariance != null)
            {
                initialVelocity.MaxSpeed += (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * (double)weapon.SpeedVariance);
            }

            Location = firedFrom.Location + (xyOffset ?? HgPoint.Zero);

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

        public virtual void ApplyIntelligence(HgPoint displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            if (X < -Settings.BulletSceneDistanceLimit
                || X >= _core.Display.TotalCanvasSize.Width + Settings.BulletSceneDistanceLimit
                || Y < -Settings.BulletSceneDistanceLimit
                || Y >= _core.Display.TotalCanvasSize.Height + Settings.BulletSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;
        }

        public override void Explode()
        {
            if (Weapon != null && Weapon.ExplodesOnImpact)
            {
                HitExplosion();
            }
            QueueForDelete();
        }
    }
}
