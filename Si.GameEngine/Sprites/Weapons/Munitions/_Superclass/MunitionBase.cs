using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Shared;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    public class MunitionBase : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public double MilisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;

        SiPoint _createdAtLocation;

        public MunitionBase(Engine gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
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
            _createdAtLocation = Location;

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
            if (Math.Abs(_createdAtLocation.X - X) > _gameCore.Settings.MunitionSceneDistanceLimit
               || Math.Abs(_createdAtLocation.Y - Y) > _gameCore.Settings.MunitionSceneDistanceLimit)
            {
                QueueForDelete();
                return;
            }

            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
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
