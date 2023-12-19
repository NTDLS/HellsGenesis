using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Weapons.BaseClasses;
using System;

namespace StrikeforceInfinity.Game.Weapons.Munitions
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public int MaxSeekingObservationDistance { get; set; } = 1000;
        public int MaxSeekingObservationAngleDegrees { get; set; } = 20;
        public int SeekingRotationRateInDegrees { get; set; } = 4;

        public SeekingMunitionBase(EngineCore gameCore, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint xyOffset = null)
            : base(gameCore, weapon, firedFrom, imagePath, xyOffset)
        {
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            if (FiredFromType == HgFiredFromType.Enemy)
            {
                if (DistanceTo(_gameCore.Player.Sprite) < MaxSeekingObservationDistance)
                {
                    var deltaAngle = DeltaAngle(_gameCore.Player.Sprite);

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
            else if (FiredFromType == HgFiredFromType.Player)
            {
                double? smallestAngle = null;

                foreach (var enemy in _gameCore.Sprites.Enemies.Visible())
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
