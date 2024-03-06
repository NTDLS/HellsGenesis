using Si.Engine.Sprites._Superclass;
using Si.Engine.Sprites.Enemies._Superclass;
using Si.Engine.Sprites.Player._Superclass;
using Si.Engine.Sprites.Weapons._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    public class MunitionBase : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public float MillisecondsToLive { get; set; } = 4000;
        public float AgeInMilliseconds => (float)(DateTime.UtcNow - CreatedDate).TotalMilliseconds;
        public float SceneDistanceLimit { get; set; }

        public MunitionBase(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(engine)
        {
            Initialize(imagePath);

            Weapon = weapon;
            Velocity.ForwardMomentium = 1.0f;
            SceneDistanceLimit = SiRandom.Between(weapon.MunitionSceneDistanceLimit * 0.1f, weapon.MunitionSceneDistanceLimit);

            RadarDotSize = new SiPoint(1, 1);

            float headingRadians = firedFrom.Velocity.Angle.Radians;
            if (weapon.AngleVarianceDegrees > 0)
            {
                var randomNumber = SiPoint.DegreesToRadians(SiRandom.Between(0, weapon.AngleVarianceDegrees * 100.0f) / 100.0f);
                headingRadians += (SiRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            float initialSpeed = weapon.Speed;
            if (Weapon.SpeedVariancePercent > 0)
            {
                var randomNumber = SiRandom.Between(0, weapon.SpeedVariancePercent * 100.0f) / 100.0f;
                var variance = randomNumber * weapon.Speed;
                initialSpeed += (SiRandom.FlipCoin() ? 1 : -1) * variance;
            }

            var initialVelocity = new SiVelocity()
            {
                Angle = new SiAngle(headingRadians),
                MaximumSpeed = initialSpeed,
                ForwardMomentium = 1.0f
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

        public virtual void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            if (!_engine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            Location += Velocity.Angle * (Velocity.MaximumSpeed * Velocity.ForwardMomentium) * epoch;
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
