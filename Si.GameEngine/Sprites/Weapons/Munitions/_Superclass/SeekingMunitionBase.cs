using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Shared.Types.Geometry;
using System;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Weapons.Munitions._Superclass
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public int MaxSeekingObservationDistance { get; set; } = 1000;
        public int MaxSeekingObservationAngleDegrees { get; set; } = 20;
        public int SeekingRotationRateInDegrees { get; set; } = 4;

        public SeekingMunitionBase(Core.Engine gameEngine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameEngine, weapon, firedFrom, imagePath, xyOffset)
        {
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
            {
                if (DistanceTo(_gameEngine.Player.Sprite) < MaxSeekingObservationDistance)
                {
                    var deltaAngle = DeltaAngle(_gameEngine.Player.Sprite);

                    if (Math.Abs((double)deltaAngle) < MaxSeekingObservationAngleDegrees)
                    {
                        if (deltaAngle >= 0) //We might as well turn around clock-wise
                        {
                            Velocity.Angle += SeekingRotationRateInDegrees;
                        }
                        else if (deltaAngle < 0) //We might as well turn around counter clock-wise
                        {
                            Velocity.Angle -= SeekingRotationRateInDegrees;
                        }
                    }
                }
            }
            else if (FiredFromType == SiFiredFromType.Player)
            {
                double? smallestAngle = null;

                foreach (var enemy in _gameEngine.Sprites.Enemies.Visible())
                {
                    if (DistanceTo(enemy) < MaxSeekingObservationDistance)
                    {
                        var deltaAngle = DeltaAngle(enemy);

                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((double)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((double)smallestAngle) < MaxSeekingObservationAngleDegrees)
                {
                    if (smallestAngle >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += SeekingRotationRateInDegrees;
                    }
                    else if (smallestAngle < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= SeekingRotationRateInDegrees;
                    }
                }
            }

            base.ApplyIntelligence(displacementVector);
        }
    }
}
