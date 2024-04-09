using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Library.Mathematics;
using System.Drawing;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebug : SpriteEnemyPeonBase
    {
        public SpriteEnemyDebug(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Debug\Hull.png");
            Throttle = 0;

            particle1 = _engine.Sprites.Particles.AddAt(SiVector.Zero, _engine.Rendering.Materials.Colors.Red, new Size(10, 10));
            particle1.Throttle = 0;
            particle1.RecalculateMovementVector();

            particle2 = _engine.Sprites.Particles.AddAt(SiVector.Zero, _engine.Rendering.Materials.Colors.Blue, new Size(10, 10));
            particle2.Throttle = 0;
            particle2.RecalculateMovementVector();
        }

        SpriteParticle particle1;
        SpriteParticle particle2;

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            //var deltaAngle = _engine.Player.Sprite.DeltaAngleInSignedDegrees(this);
            //var unsigned = _engine.Player.Sprite.Location.AngleToInSignedRadians(Location);
            //System.Diagnostics.Debug.WriteLine($"DeltaAngle: {deltaAngle:n4}");

            Orientation.DegreesSigned += 1;

            var point1 = Orientation.RotatedBy(SiMath.RADIANS_90).PointFromAngleAtDistance(new SiVector(50, 50));
            particle1.Location = Location + point1;

            var point2 = Orientation.RotatedBy(-SiMath.RADIANS_90).PointFromAngleAtDistance(new SiVector(50, 50));
            particle2.Location = Location + point2;

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}

