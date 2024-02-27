using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Utility;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Types;
using Si.Library.Types.Geometry;
using System;
using static Si.Library.SiConstants;

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
        public double MillisecondsToLive { get; set; } = 4000;
        public double AgeInMilliseconds => (DateTime.UtcNow - CreatedDate).TotalMilliseconds;
        public double SceneDistanceLimit { get; set; }

        public MunitionBase(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameEngine)
        {
            Initialize(imagePath);

            Weapon = weapon;
            Velocity.ThrottlePercentage = 1.0;
            SceneDistanceLimit = SiRandom.Between(weapon.MunitionSceneDistanceLimit * 0.1, weapon.MunitionSceneDistanceLimit);

            RadarDotSize = new SiPoint(1, 1);

            double headingRadians = firedFrom.Velocity.Angle.Radians;
            if (weapon.AngleVarianceDegrees > 0)
            {
                var randomNumber = SiSpriteVectorMath.DegreesToRadians(SiRandom.Between(0, weapon.AngleVarianceDegrees * 100.0) / 100.0);
                headingRadians += (SiRandom.FlipCoin() ? 1 : -1) * randomNumber;
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
                Angle = new SiAngle(headingRadians),
                Speed = initialSpeed,
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

        public virtual void ApplyIntelligence(double epoch, SiPoint displacementVector)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(double epoch, SiPoint displacementVector)
        {
            if (!_gameEngine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            Location += Velocity.Angle * (Velocity.Speed * Velocity.ThrottlePercentage) * epoch;
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
