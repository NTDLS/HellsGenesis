using SharpDX;
using SharpDX.Direct2D1;
using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteParticle : SpriteParticleBase
    {
        /// <summary>
        /// The max travel distance from the creation x,y before the sprite is automatically deleted.
        /// This is ignored unless the CleanupModeOption is Distance.
        /// </summary>
        public double MaxDistance { get; set; } = 1000;

        /// <summary>
        /// The amount of brightness to reduce the color by each time the particle is rendered.
        /// This is ignored unless the CleanupModeOption is FadeToBlack.
        /// This should be expressed as a number between 0-1 with 0 being no reduxtion per frame and 1 being 100% reduction per frame.
        /// </summary>
        public float FadeToBlackReductionAmount { get; set; } = 0.01f;

        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.Native;
        public ParticleShape Shape { get; set; } = ParticleShape.FilledEllipse;
        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;
        public double RotationSpeed { get; set; } = 0;
        public SiRelativeDirection RotationDirection { get; set; } = SiRelativeDirection.None;
        public Color4 Color { get; set; }
        public SiAngle TravelAngle { get; set; } = new SiAngle();
        private readonly SiPoint _originalLocation;

        public SpriteParticle(GameEngineCore gameEngine, double x, double y, Size size, Color4 color)
            : base(gameEngine)
        {
            Initialize(size);

            X = x;
            Y = y;

            _originalLocation = new SiPoint(x, y);

            Color = color;
            RotationSpeed = SiRandom.Between(1, 100) / 20.0;
            RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
            TravelAngle.Degrees = SiRandom.Between(0, 359);

            Velocity.ThrottlePercentage = 100;
            Velocity.MaxSpeed = SiRandom.Between(1.0, 4.0);

            _gameEngine = gameEngine;
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            if (RotationDirection == SiRelativeDirection.Right)
            {
                Velocity.Angle.Degrees += RotationSpeed;
            }
            else if (RotationDirection == SiRelativeDirection.Left)
            {
                Velocity.Angle.Degrees -= RotationSpeed;
            }

            if (VectorType == ParticleVectorType.Independent)
            {
                //We use a seperate angle for the travel direction because the base ApplyMotion()
                //  moves the object in the the direction of the Velocity.Angle.
                X += TravelAngle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
                Y += TravelAngle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
            }
            else if (VectorType == ParticleVectorType.Native)
            {
                base.ApplyMotion(displacementVector);
            }

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                Color *= 1 - FadeToBlackReductionAmount; // Gradually darken the particle color.

                // Check if the particle color is below a certain threshold and remove it.
                if (Color.Red < 0.1f && Color.Green < 0.1f && Color.Blue < 0.1f)
                {
                    QueueForDelete();
                }
            }
            else if (CleanupMode == ParticleCleanupMode.Distance)
            {
                if (DistanceTo(_originalLocation) > MaxDistance)
                {
                    QueueForDelete();
                }
            }
        }

        public override void Render(RenderTarget renderTarget)
        {
            if (Visable)
            {
                switch (Shape)
                {
                    case ParticleShape.FilledEllipse:
                        _gameEngine.Rendering.FillEllipseAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Velocity.Angle.Degrees);
                        break;
                    case ParticleShape.HollowEllipse:
                        _gameEngine.Rendering.HollowEllipseAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Velocity.Angle.Degrees);
                        break;
                    case ParticleShape.Triangle:
                        _gameEngine.Rendering.HollowTriangleAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Velocity.Angle.Degrees);
                        break;
                }
            }
        }
    }
}
