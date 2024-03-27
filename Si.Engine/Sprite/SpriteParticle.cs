using SharpDX;
using SharpDX.Direct2D1;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteParticle : SpriteParticleBase
    {
        /// <summary>
        /// The max travel distance from the creation x,y before the sprite is automatically deleted.
        /// This is ignored unless the CleanupModeOption is Distance.
        /// </summary>
        public float MaxDistance { get; set; } = 1000;

        /// <summary>
        /// The amount of brightness to reduce the color by each time the particle is rendered.
        /// This is ignored unless the CleanupModeOption is FadeToBlack.
        /// This should be expressed as a number between 0-1 with 0 being no reduxtion per frame and 1 being 100% reduction per frame.
        /// </summary>
        public float FadeToBlackReductionAmount { get; set; } = 0.01f;

        public ParticleColorType ColorType { get; set; } = ParticleColorType.SingleColor;
        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.UseNativeForwardAngle;
        public ParticleShape Shape { get; set; } = ParticleShape.FilledEllipse;
        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;

        /// <summary>
        /// The color of the particle when ColorType == Color;
        /// </summary>
        public Color4 Color { get; set; }

        /// <summary>
        /// The color of the particle when ColorType == Graident;
        /// </summary>
        public Color4 GradientStartColor { get; set; }
        /// <summary>
        /// The color of the particle when ColorType == Graident;
        /// </summary>
        public Color4 GradientEndColor { get; set; }

        /// <summary>
        /// Allow for a seperate angle for the travel direction because we want the sprite to travel in a direction that is is not pointing.
        /// </summary>
        public SiAngle TravelAngle { get; set; } = new SiAngle();

        public SpriteParticle(EngineCore engine, SiPoint location, Size size, Color4? color = null)
            : base(engine)
        {
            SetSize(size);

            Location = location.Clone();

            Color = color ?? engine.Rendering.Materials.Colors.White;
            RotationSpeed = SiRandom.Between(1, 100) / 20.0f * SiRandom.PositiveOrNegative();
            TravelAngle.Degrees = SiRandom.Between(0, 359);

            Travel.Velocity = VelocityInDirection(1.0f);
            Travel.MaximumSpeed = SiRandom.Between(1.0f, 4.0f);

            _engine = engine;
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Direction.Degrees -= RotationSpeed * epoch;

            if (VectorType == ParticleVectorType.UseTravelAngle)
            {
                //We use a seperate angle for the travel direction because we want the sprite to travel in a direction that is is not pointing.
                Location += TravelAngle * Travel.MaximumSpeed * epoch;
            }
            else if (VectorType == ParticleVectorType.UseNativeForwardAngle)
            {
                base.ApplyMotion(epoch, displacementVector);
            }

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                if (ColorType == ParticleColorType.SingleColor)
                {
                    Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
                else if (ColorType == ParticleColorType.Graident)
                {
                    GradientStartColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.
                    GradientEndColor *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (GradientStartColor.Red < 0.5f && GradientStartColor.Green < 0.5f && GradientStartColor.Blue < 0.5f
                        || GradientEndColor.Red < 0.5f && GradientEndColor.Green < 0.5f && GradientEndColor.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
            }
            else if (CleanupMode == ParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
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
                        if (ColorType == ParticleColorType.SingleColor)
                        {
                            _engine.Rendering.FillEllipseAt(renderTarget,
                                RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Direction.Degrees);
                        }
                        else if (ColorType == ParticleColorType.Graident)
                        {
                            _engine.Rendering.FillEllipseAt(renderTarget, RenderLocation.X, RenderLocation.Y,
                                Size.Width, Size.Height, GradientStartColor, GradientEndColor, (float)Direction.Degrees);
                        }
                        break;
                    case ParticleShape.HollowEllipse:
                        _engine.Rendering.HollowEllipseAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Direction.Degrees);
                        break;
                    case ParticleShape.Triangle:
                        _engine.Rendering.HollowTriangleAt(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Direction.Degrees);
                        break;
                }

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangleAt(renderTarget, RawRenderBounds, Direction.Radians, _engine.Rendering.Materials.Colors.Red, 0, 1);
                }
            }
        }
    }
}
