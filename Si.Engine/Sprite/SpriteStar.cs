using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.Engine.Sprite
{
    public class SpriteStar : SpriteBase
    {
        private const string _assetPath = @"Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteStar(EngineCore engine)
            : base(engine)
        {
            selectedImageIndex = SiRandom.Between(0, _imageCount - 1);
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);

            Velocity.MaximumSpeed = 0.5f;

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ForwardMomentium = SiRandom.Between(8, 10) / 10.0f;
            }
            else
            {
                Velocity.ForwardMomentium = SiRandom.Between(4, 8) / 10.0f;
            }
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Location -= displacementVector * Velocity.MaximumSpeed * Velocity.ForwardMomentium * epoch;
        }
    }
}
