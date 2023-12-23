using Si.Game.Engine;
using Si.Game.Engine.Types;
using Si.Game.Sprites;
using Si.Game.Sprites.Enemies.BasesAndInterfaces;
using Si.Game.Sprites.Player.BasesAndInterfaces;
using Si.Game.Utility;
using Si.Game.Weapons.BasesAndInterfaces;
using System;
using static Si.Shared.SiConstants;
using Si.Shared;
using Si.Shared.Types.Geometry;
using Si.Shared.Types;

namespace Si.Game.Weapons.Munitions
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    internal class MunitionBase : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        public MunitionBase(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameCore)
        {
            Initialize(imagePath);

            Weapon = weapon;
            Velocity.ThrottlePercentage = 1.0;

            RadarDotSize = new SiPoint(1, 1);

            double headingDegrees = firedFrom.Velocity.Angle.Degrees;
            if (weapon.AngleVarianceDegrees > 0)
            {
                var randomNumber = SiRandom.Between(0, weapon.AngleVarianceDegrees * 100.0) / 100.0;
                headingDegrees += (SiRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            double initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = SiRandom.Between(0, weapon.SpeedVariancePercent * 100.0) / 100.0;
                var variance = randomNumber * weapon.Speed;
                initialSpeed += (SiRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new SiVelocity()
            {
                Angle = new SiAngle(headingDegrees),
                MaxSpeed = initialSpeed,
                ThrottlePercentage = 1.0
            };

            Location = firedFrom.Location + (xyOffset ?? SiPoint.Zero);

            if (firedFrom is SpriteEnemyBase)
            {
                FiredFromType = SiFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayerBase)
            {
                FiredFromType = SiFiredFromType.Player;
            }

            Velocity = initialVelocity;
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            if (AgeInMilliseconds > MilisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (X < -_gameCore.Settings.MunitionSceneDistanceLimit
                || X >= _gameCore.Display.TotalCanvasSize.Width + _gameCore.Settings.MunitionSceneDistanceLimit
                || Y < -_gameCore.Settings.MunitionSceneDistanceLimit
                || Y >= _gameCore.Display.TotalCanvasSize.Height + _gameCore.Settings.MunitionSceneDistanceLimit)
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
