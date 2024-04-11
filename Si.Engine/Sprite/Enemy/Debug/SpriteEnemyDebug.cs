using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Library.ExtensionMethods;
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

            _particle1 = _engine.Sprites.Particles.AddAt(SiVector.Zero, new Size(5, 5));
            _particle1.Pattern = Library.SiConstants.ParticleColorType.Solid;
            _particle1.Color = _engine.Rendering.Materials.Colors.Red;
            _particle1.Shape = Library.SiConstants.ParticleShape.HollowEllipse;
            _particle1.Throttle = 0;
            _particle1.RotationSpeed = 0;
            _particle1.RecalculateMovementVector();

            _particle2 = _engine.Sprites.Particles.AddAt(SiVector.Zero, new Size(5, 5));
            _particle2.Pattern = Library.SiConstants.ParticleColorType.Solid;
            _particle2.Color = _engine.Rendering.Materials.Colors.Green;
            _particle2.Shape = Library.SiConstants.ParticleShape.FilledEllipse;
            _particle2.Throttle = 0;
            _particle2.RotationSpeed = 0;
            _particle2.RecalculateMovementVector();

            _particle3 = _engine.Sprites.Particles.AddAt(SiVector.Zero, new Size(10, 10));
            _particle3.Pattern = Library.SiConstants.ParticleColorType.Solid;
            _particle3.Color = _engine.Rendering.Materials.Colors.Blue;
            _particle3.Shape = Library.SiConstants.ParticleShape.HollowRectangle;
            _particle3.Throttle = 0;
            _particle3.RotationSpeed = 0.02f;
            _particle3.RecalculateMovementVector();

            _particle4 = _engine.Sprites.Particles.AddAt(SiVector.Zero, new Size(10, 10));
            _particle4.Pattern = Library.SiConstants.ParticleColorType.Solid;
            _particle4.Color = _engine.Rendering.Materials.Colors.Cyan;
            _particle4.Shape = Library.SiConstants.ParticleShape.Triangle;
            _particle4.Throttle = 0;
            _particle4.RotationSpeed = 0.02f;
            _particle4.RecalculateMovementVector();
        }

        SpriteParticle _particle1;
        SpriteParticle _particle2;
        SpriteParticle _particle3;
        SpriteParticle _particle4;

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            Orientation.RadiansSigned += 0.05f; // = this.AngleToInSignedRadians(_engine.Player.Sprite);

            var point1 = Orientation.RotatedBy(90.ToRadians()) * new SiVector(50, 50);
            _particle1.Location = Location + point1;

            var point2 = Orientation.RotatedBy(-90.ToRadians()) * new SiVector(50, 50);
            _particle2.Location = Location + point2;

            var point3 = Orientation * new SiVector(50, 50);
            _particle3.Location = Location + point3;

            var point4 = Orientation * new SiVector(50, 50) * -1;
            _particle4.Location = Location + point4;

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}

