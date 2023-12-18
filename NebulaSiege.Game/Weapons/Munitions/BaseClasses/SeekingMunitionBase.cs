using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Weapons.BaseClasses;
using System;

namespace NebulaSiege.Game.Weapons.Munitions
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public int MaxSeekingObservationDistance { get; set; } = 1000;
        public int MaxSeekingObservationAngleDegrees { get; set; } = 20;
        public int SeekingRotationRateInDegrees { get; set; } = 4;

        public SeekingMunitionBase(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (FiredFromType == HgFiredFromType.Enemy)
            {
                if (DistanceTo(_core.Player.Sprite) < MaxSeekingObservationDistance)
                {
                    var deltaAngle = DeltaAngle(_core.Player.Sprite);

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

                foreach (var enemy in _core.Sprites.Enemies.Visible())
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
