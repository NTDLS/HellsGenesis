using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.IO;

namespace Si.GameEngine.Sprites
{
    public class SpriteStar : SpriteBase
    {
        private const string _assetPath = @"Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteStar(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            selectedImageIndex = SiRandom.Between(0, _imageCount - 1);
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = SiRandom.Between(0, gameEngine.Display.TotalCanvasSize.Width);
            Y = SiRandom.Between(0, gameEngine.Display.TotalCanvasSize.Height);

            Velocity.Speed = 0.5f;

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ThrottlePercentage = SiRandom.Between(8, 10) / 10.0f;
            }
            else
            {
                Velocity.ThrottlePercentage = SiRandom.Between(4, 8) / 10.0f;
            }
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Location -= displacementVector * Velocity.Speed * Velocity.ThrottlePercentage * epoch;
        }
    }
}
