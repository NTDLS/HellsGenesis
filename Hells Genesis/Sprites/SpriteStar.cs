using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Utility;
using System.IO;

namespace HG.Sprites
{
    internal class SpriteStar : SpriteBase
    {
        private const string _assetPath = @"Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public SpriteStar(EngineCore core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % _imageCount;
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = HgRandom.Generator.Next(0, core.Display.TotalCanvasSize.Width);
            Y = HgRandom.Generator.Next(0, core.Display.TotalCanvasSize.Height);

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ThrottlePercentage = HgRandom.Generator.Next(8, 10) / 10.0;
            }
            else
            {
                Velocity.ThrottlePercentage = HgRandom.Generator.Next(4, 8) / 10.0;
            }
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            X -= displacementVector.X * Velocity.ThrottlePercentage;
            Y -= displacementVector.Y * Velocity.ThrottlePercentage;
        }
    }
}
