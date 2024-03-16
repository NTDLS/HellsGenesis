using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.Engine.Sprite
{
    public class SpriteNebula : SpriteBase
    {
        private const string _assetPath = @"Sprites\LargeNebula\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteNebula(EngineCore engine)
            : base(engine)
        {
            selectedImageIndex = SiRandom.Between(0, _imageCount - 1);
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);

            ZOrder = int.MinValue;

            Velocity.MaximumSpeed = 0.10f;

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ForwardVelocity = SiRandom.Between(8, 10) / 10.0f;
            }
            else
            {
                Velocity.ForwardVelocity = SiRandom.Between(4, 8) / 10.0f;
            }
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            if (displacementVector.Sum() != 0)
            {
                //Location -= displacementVector;// - (displacementVector.Normalize() * Velocity.MaximumSpeed);
                Location += displacementVector - (displacementVector.Normalize() * Velocity.MaximumSpeed);
            }
        }
    }
}
