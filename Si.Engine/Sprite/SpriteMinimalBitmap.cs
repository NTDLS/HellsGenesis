﻿using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.SupportingClasses.Metadata;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    /// <summary>
    /// These are just minimal non-collidable, non interactive, generic bitmap sprites.
    /// </summary>
    public class SpriteMinimalBitmap : SpriteBase
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

        public ParticleVectorType VectorType { get; set; } = ParticleVectorType.Default;

        public ParticleCleanupMode CleanupMode { get; set; } = ParticleCleanupMode.None;

        public SpriteMinimalBitmap(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
        }

        public SpriteMinimalBitmap(EngineCore engine, SharpDX.Direct2D1.Bitmap bitmap)
            : base(engine)
        {
            SetImage(bitmap);
        }

        public SpriteMinimalBitmap(EngineCore engine)
            : base(engine)
        {
        }

        private void SetImageAndLoadMetadata(string spriteImagePath)
        {
            var metadata = _engine.Assets.GetMetaData<SpriteMinimalBitmapMetadata>(spriteImagePath);

            Speed = metadata.Speed;
            Throttle = metadata.Throttle;
            MaxThrottle = metadata.MaxThrottle;

            SetImage(spriteImagePath);
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
