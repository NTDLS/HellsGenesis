using HG.Actors.BaseClasses;
using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Ordinary;
using HG.Actors.Weapons.BaseClasses;
using HG.Engine;
using HG.Types;
using System;
using System.Drawing;

namespace HG.Actors.Weapons.Bullets.BaseClasses
{
    internal class BulletBase : ActorBase
    {
        public HgFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public ActorBase LockedTarget { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
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

            var initialVelocity = new HgVelocity<double>()
            {
                Angle = new HgAngle<double>(headingDegrees),
                MaxSpeed = weapon.Speed,
                ThrottlePercentage = 100
            };

            if (Weapon.SpeedSlop != null)
            {
                initialVelocity.MaxSpeed += (HgRandom.FlipCoin() ? 1 : -1) * (HgRandom.Random.NextDouble() * (double)weapon.AngleSlop);
            }

            Location = firedFrom.Location + (xyOffset == null ? HgPoint<double>.Zero : xyOffset);

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

        public virtual void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(HgPoint<double> displacementVector)
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
