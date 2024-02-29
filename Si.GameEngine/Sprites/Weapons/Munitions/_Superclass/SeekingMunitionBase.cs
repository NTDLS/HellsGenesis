using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public int MaxSeekingObservationDistance { get; set; } = 1000;
        public int MaxSeekingObservationAngleDegrees { get; set; } = 20;
        public float SeekingRotationRateRadians { get; set; } = SiVector.DegreesToRadians(4);

        public SeekingMunitionBase(GameEngineCore gameEngine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiVector xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
            {
                if (DistanceTo(_gameEngine.Player.Sprite) < MaxSeekingObservationDistance)
                {
                    var deltaAngle = DeltaAngleDegrees(_gameEngine.Player.Sprite);

                    if (Math.Abs((float)deltaAngle) < MaxSeekingObservationAngleDegrees)
                    {
                        if (deltaAngle >= 0) //We might as well turn around clock-wise
                        {
                            Velocity.Angle += SeekingRotationRateRadians;
                        }
                        else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                        {
                            Velocity.Angle -= SeekingRotationRateRadians;
                        }
                    }
                }
            }
            else if (FiredFromType == SiFiredFromType.Player)
            {
                float? smallestAngle = null;

                foreach (var enemy in _gameEngine.Sprites.Enemies.Visible())
                {
                    if (DistanceTo(enemy) < MaxSeekingObservationDistance)
                    {
                        var deltaAngle = DeltaAngleDegrees(enemy);

                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((float)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((float)smallestAngle) < MaxSeekingObservationAngleDegrees)
                {
                    if (smallestAngle >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += SeekingRotationRateRadians;
                    }
                    else if (smallestAngle < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= SeekingRotationRateRadians;
                    }
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
