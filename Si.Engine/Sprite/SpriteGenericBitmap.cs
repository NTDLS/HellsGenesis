using Si.Engine.Sprite._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteGenericBitmap : SpriteInteractiveBase
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

        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.UseNativeForwardAngle;
        /// <summary>
        /// Allow for a seperate angle for the travel direction because we want the sprite to travel in a direction that is is not pointing.
        /// </summary>
        public SiVector TravelAngle { get; set; } = new SiVector();
        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;

        public SpriteGenericBitmap(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            SetImage(bitmap);
        }

        public SpriteGenericBitmap(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Direction.Degrees += RotationSpeed * epoch;

            if (VectorType == ParticleVectorType.UseTravelAngle)
            {
                //We use a seperate angle for the travel direction because we want the sprite to travel in a direction that is is not pointing.
                //Location += TravelAngle * Speed * epoch;

                base.ApplyMotion(epoch, displacementVector);
            }
            else if (VectorType == ParticleVectorType.UseNativeForwardAngle)
            {
                base.ApplyMotion(epoch, displacementVector);
            }

            if (CleanupMode == ParticleCleanupMode.FadeToBlack)
            {
                throw new NotImplementedException();
                /*
                Color *= 1 - (float)FadeToBlackReductionAmount; // Gradually darken the particle color.

                // Check if the particle color is below a certain threshold and remove it.
                if (Color.Red < 0.5f && Color.Green < 0.5f && Color.Blue < 0.5f)
                {
                    QueueForDelete();
                }
                */
            }
            else if (CleanupMode == ParticleCleanupMode.DistanceOffScreen)
            {
                if (_engine.Display.TotalCanvasBounds.Balloon(MaxDistance).IntersectsWith(RenderBounds) == false)
                {
                    QueueForDelete();
                }
            }
        }
    }
}
