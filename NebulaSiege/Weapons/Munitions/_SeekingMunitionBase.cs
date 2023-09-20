using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;
using System;

namespace HellsGenesis.Weapons.Munitions
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class _SeekingMunitionBase : _MunitionBase
    {
        public int MaxObservationDistance { get; set; } = 1000;
        public int MaxObservationAngleDegrees { get; set; } = 20;
        public int RotationRateInDegrees { get; set; } = 4;

        public _SeekingMunitionBase(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, string imagePath, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (FiredFromType == HgFiredFromType.Player)
            {
                double? smallestAngle = null;

                foreach (var enemy in _core.Sprites.Enemies.Visible())
                {
                    if (enemy.DistanceTo(_core.Player.Sprite) < MaxObservationDistance)
                    {
                        var deltaAngle = DeltaAngle(enemy);

                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((double)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((double)smallestAngle) < MaxObservationAngleDegrees)
                {
                    if (smallestAngle >= 0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += RotationRateInDegrees;
                    }
                    else if (smallestAngle < 0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= RotationRateInDegrees;
                    }
                }
            }

            base.ApplyIntelligence(displacementVector);
        }
    }
}
