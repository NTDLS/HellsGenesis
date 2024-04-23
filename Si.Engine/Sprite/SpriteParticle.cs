using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
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
        public ParticleColorType Pattern { get; set; } = ParticleColorType.Solid;
        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.Default;
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

        public SpriteParticle(EngineCore engine, SiVector location, Size size, Color4? color = null)
            : base(engine)
        {
            SetSize(size);

            Location = location.Clone();

            Color = color ?? engine.Rendering.Materials.Colors.White;
            RotationSpeed = SiRandom.Between(0.01f, 0.09f) * SiRandom.PositiveOrNegative();

            Speed = SiRandom.Between(1.0f, 4.0f);
            Orientation.Degrees = SiRandom.Between(0, 359);
            Throttle = 1;

            RecalculateMovementVector();

            _engine = engine;
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            Orientation.Degrees += RotationSpeed * epoch;

            if (VectorType == ParticleVectorType.FollowOrientation)
            {
                RecalculateMovementVector(Orientation.RadiansSigned);
            }

            base.ApplyMotion(epoch, displacementVector);

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                if (Pattern == ParticleColorType.Solid)
                {
                    Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                    // Check if the particle color is below a certain threshold and remove it.
                    if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                    {
                        QueueForDelete();
                    }
                }
                else if (Pattern == ParticleColorType.Graident)
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
            if (Visible)
            {
                switch (Shape)
                {
                    case ParticleShape.FilledEllipse:
                        if (Pattern == ParticleColorType.Solid)
                        {
                            _engine.Rendering.DrawSolidEllipse(renderTarget,
                                RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, (float)Orientation.Degrees);
                        }
                        else if (Pattern == ParticleColorType.Graident)
                        {
                            _engine.Rendering.DrawGradientEllipse(renderTarget, RenderLocation.X, RenderLocation.Y,
                                Size.Width, Size.Height, GradientStartColor, GradientEndColor, (float)Orientation.Degrees);
                        }
                        break;
                    case ParticleShape.HollowEllipse:
                        _engine.Rendering.DrawEllipse(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;

                    case ParticleShape.FilledRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);

                            if (Pattern == ParticleColorType.Solid)
                            {
                                _engine.Rendering.DrawSolidRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, Color, 0, (float)Orientation.Degrees);
                            }
                            else if (Pattern == ParticleColorType.Graident)
                            {
                                _engine.Rendering.DrawGradientRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                    RenderLocation.Y - Size.Height / 2, rect, GradientStartColor, GradientEndColor, 0, (float)Orientation.Degrees);
                            }
                        }
                        break;

                    case ParticleShape.HollowRectangle:
                        {
                            var rect = new RawRectangleF(0, 0, Size.Width, Size.Height);
                            _engine.Rendering.DrawRectangle(renderTarget, RenderLocation.X - Size.Width / 2,
                                RenderLocation.Y - Size.Height / 2, rect, Color, 0, 1, (float)Orientation.Degrees);

                        }
                        break;

                    case ParticleShape.Triangle:
                        _engine.Rendering.DrawTriangle(renderTarget,
                            RenderLocation.X, RenderLocation.Y, Size.Width, Size.Height, Color, 1, (float)Orientation.Degrees);
                        break;
                }

                if (IsHighlighted)
                {
                    _engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        _engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }
            }
        }
    }
}
