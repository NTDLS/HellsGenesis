using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies;
using NebulaSiege.Sprites.Player;
using NebulaSiege.Utility;
using System;

namespace NebulaSiege.Weapons.Munitions
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    internal class _MunitionBase : _SpriteBase
    {
        public HgFiredFromType FiredFromType { get; private set; }
        public _WeaponBase Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        public _MunitionBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core)
        {
            Initialize(imagePath);

            Weapon = weapon;
            Velocity.ThrottlePercentage = 1.0;

            RadarDotSize = new NsPoint(1, 1);

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
                Angle = new NsAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

            Location = firedFrom.Location + (xyOffset ?? NsPoint.Zero);

            if (firedFrom is _SpriteEnemyBase)
            {
                FiredFromType = HgFiredFromType.Enemy;
            }
            else if (firedFrom is _SpritePlayerBase)
            {
                FiredFromType = HgFiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(NsPoint displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(NsPoint displacementVector)
        {
            if (X < -_core.Settings.MunitionSceneDistanceLimit
                || X >= _core.Display.TotalCanvasSize.Width + _core.Settings.MunitionSceneDistanceLimit
                || Y < -_core.Settings.MunitionSceneDistanceLimit
                || Y >= _core.Display.TotalCanvasSize.Height + _core.Settings.MunitionSceneDistanceLimit)
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
