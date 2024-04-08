using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
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

        /// <summary>
        /// Creates a munition for the given weapon.
        /// </summary>
        /// <param name="engine">Reference to the engine.</param>
        /// <param name="weapon">The weapon to create a munition for.</param>
        /// <param name="firedFrom">The sprite that is firing the weapon.</param>
        /// <param name="imagePath">The image for the munition.</param>
        /// <param name="location">The optional location for the munition to originate from (if not specified, wull use the location of the firedFrom sprite).</param>
        /// <param name="angle">>The optional angle for the munition to travel on (if not specified, wull use the angle of the firedFrom sprite).</param>
        public MunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string imagePath, SiVector location = null, float? angle = null)
            : base(engine)
        {
            if (imagePath != null)
            {
                SetImage(imagePath);
            }
            else
            {
                //The image is likely being set in the derived class.
            }

            Weapon = weapon;
            RadarDotSize = new SiVector(1, 1);
            SceneDistanceLimit = SiRandom.Between(weapon.Metadata.MunitionSceneDistanceLimit * 0.1f, weapon.Metadata.MunitionSceneDistanceLimit);

            float headingRadians = angle == null ? firedFrom.PointingAngle.Radians : (float)angle;
            if (weapon.Metadata.AngleVarianceDegrees > 0)
            {
                var randomNumber = SiVector.DegToRad(SiRandom.Between(0, weapon.Metadata.AngleVarianceDegrees * 100.0f) / 100.0f);
                headingRadians += (SiRandom.FlipCoin() ? 1 : -1) * randomNumber;
            }

            float initialSpeed = weapon.Metadata.Speed;
            if (weapon.Metadata.SpeedVariancePercent > 0)
            {
                var randomNumber = SiRandom.Between(0, weapon.Metadata.SpeedVariancePercent * 100.0f) / 100.0f;
                var variance = randomNumber * weapon.Metadata.Speed;
                initialSpeed += (SiRandom.FlipCoin() ? 1 : -1) * variance;
            }

            Location = location ?? firedFrom.Location;
            PointingAngle = new SiAngle(headingRadians);
            Speed = initialSpeed;
            MovementVector = PointingAngle * initialSpeed;

            if (firedFrom is SpriteAttachment attachment)
            {
                //If we are firing from an attachment, get the type of the root owner.
                firedFrom = attachment.OwnerSprite;
            }

            if (firedFrom is SpriteEnemyBase)
            {
                FiredFromType = SiFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayerBase)
            {
                FiredFromType = SiFiredFromType.Player;
            }
            else
            {
                throw new Exception($"Munitions for {firedFrom.GetType().Name} are not implemented.");
            }
        }

        public virtual void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (!_engine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            Location += MovementVector * epoch;
        }

        public override void Explode()
        {
            if (Weapon != null && Weapon.Metadata.ExplodesOnImpact)
            {
                HitExplosion();
            }
            QueueForDelete();
        }
    }
}
