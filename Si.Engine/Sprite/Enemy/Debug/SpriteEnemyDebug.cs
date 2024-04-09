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

            particle1 = _engine.Sprites.Particles.AddAt(SiVector.Zero, new Size(5, 10));
            particle1.ColorType = Library.SiConstants.ParticleColorType.Graident;
            particle1.GradientStartColor = _engine.Rendering.Materials.Colors.Red;
            particle1.GradientEndColor = _engine.Rendering.Materials.Colors.Green;
            particle1.Throttle = 0;
            particle1.RecalculateMovementVector();

            particle2 = _engine.Sprites.Particles.AddAt(SiVector.Zero, _engine.Rendering.Materials.Colors.Blue, new Size(5, 10));
            particle2.ColorType = Library.SiConstants.ParticleColorType.Graident;
            particle2.GradientStartColor = _engine.Rendering.Materials.Colors.Cyan;
            particle2.GradientEndColor = _engine.Rendering.Materials.Colors.Blue;
            particle2.Throttle = 0;
            particle2.RecalculateMovementVector();

            particle3 = _engine.Sprites.Particles.AddAt(SiVector.Zero, _engine.Rendering.Materials.Colors.Orange, new Size(5, 10));
            particle3.ColorType = Library.SiConstants.ParticleColorType.Graident;
            particle3.GradientStartColor = _engine.Rendering.Materials.Colors.Orange;
            particle3.GradientEndColor = _engine.Rendering.Materials.Colors.LawnGreen;
            particle3.Throttle = 0;
            particle3.RecalculateMovementVector();

            particle4 = _engine.Sprites.Particles.AddAt(SiVector.Zero, _engine.Rendering.Materials.Colors.Green, new Size(5, 10));
            particle4.ColorType = Library.SiConstants.ParticleColorType.Graident;
            particle4.GradientStartColor = _engine.Rendering.Materials.Colors.OrangeRed;
            particle4.GradientEndColor = _engine.Rendering.Materials.Colors.Cyan;
            particle4.Throttle = 0;
            particle4.RecalculateMovementVector();
        }

        SpriteParticle particle1;
        SpriteParticle particle2;
        SpriteParticle particle3;
        SpriteParticle particle4;

        float i = 0;

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            Orientation.RadiansSigned = this.AngleToInSignedRadians(_engine.Player.Sprite);

            var point1 = Orientation.RotatedBy(SiMath.RADIANS_90 + i).PointFromAngleAtDistance(new SiVector(50, 50));
            particle1.Location = Location + point1;

            var point2 = Orientation.RotatedBy(-SiMath.RADIANS_90 + i).PointFromAngleAtDistance(new SiVector(50, 50));
            particle2.Location = Location + point2;

            var point3 = Orientation.RotatedBy(SiMath.RADIANS_90 - i).PointFromAngleAtDistance(new SiVector(80, 80));
            particle3.Location = Location + point3;

            var point4 = Orientation.RotatedBy(-SiMath.RADIANS_90 - i).PointFromAngleAtDistance(new SiVector(80, 80));
            particle4.Location = Location + point4;

            i += 0.01f;

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}

