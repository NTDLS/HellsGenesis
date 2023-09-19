using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Sprites.Enemies;
using HG.Utility;
using System;

namespace HG.Weapons.Bullets
{
    internal class BulletBase : SpriteBase
    {
        public HgFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public SpriteBase LockedTarget { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        public BulletBase(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, string imagePath,
             SpriteBase lockedTarget = null, HgPoint xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            LockedTarget = lockedTarget;
            Velocity.ThrottlePercentage = 1.0;

            RadarDotSize = new HgPoint(1, 1);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;
            if (weapon.AngleVarianceDegrees > 0)
            {
                var randomNumber = HgRandom.Between(0, weapon.AngleVarianceDegrees * 100.0) / 100.0;
                headingDegrees += (HgRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            double initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = HgRandom.Between(0, weapon.SpeedVariancePercent * 100.0) / 100.0;
                var variance = randomNumber * weapon.Speed;
                initialSpeed += (HgRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new HgVelocity()
            {
                Angle = new HgAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

            Location = firedFrom.Location + (xyOffset ?? HgPoint.Zero);

            if (firedFrom is SpriteEnemyBase)
            {
                FiredFromType = HgFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayer)
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
