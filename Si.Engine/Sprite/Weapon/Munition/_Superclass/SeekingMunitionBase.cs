using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public int MaxSeekingObservationDistance { get; set; } = 1000;
        public int MaxSeekingObservationAngleDegrees { get; set; } = 20;
        public float SeekingRotationRateRadians { get; set; } = SiMath.DegToRad(4);

        public SeekingMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string imagePath, SiVector location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
            {
                if (DistanceTo(_engine.Player.Sprite) < MaxSeekingObservationDistance)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);

                    if (Math.Abs((float)deltaAngle) < MaxSeekingObservationAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(SeekingRotationRateRadians * (deltaAngle > 0 ? 1 : -1));
                    }
                }
            }
            else if (FiredFromType == SiFiredFromType.Player)
            {
                float? smallestAngle = null;

                foreach (var enemy in _engine.Sprites.Enemies.Visible())
                {
                    if (DistanceTo(enemy) < MaxSeekingObservationDistance)
                    {
                        var deltaAngle = this.HeadingAngleToInSignedDegrees(enemy);
                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((float)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((float)smallestAngle) < MaxSeekingObservationAngleDegrees && smallestAngle.IsNearZero())
                {
                    RotateMovementVector(SeekingRotationRateRadians * (smallestAngle > 0 ? 1 : -1));
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
