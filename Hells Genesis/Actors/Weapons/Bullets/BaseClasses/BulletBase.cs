using HG.Actors.BaseClasses;
using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Ordinary;
using HG.Actors.Weapons.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Types.Geometry;
using HG.Utility;
using System;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

            RadarDotSize = new HgPoint(1, 1);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;
            if (weapon.AngleVariancePercent > 0)
            {
                var randomNumber = HgRandom.RandomNumber(0, weapon.AngleVariancePercent);
                var variance = (randomNumber / 100) * firedFrom.Velocity.Angle.Degrees;
                headingDegrees += (HgRandom.FlipCoin() ? 1 : -1) * variance;
            }

            double initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = HgRandom.RandomNumber(0, weapon.SpeedVariancePercent);
                var variance = (randomNumber / 100) * weapon.Speed;
                initialSpeed += (HgRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new HgVelocity()
            {
                Angle = new HgAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

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
