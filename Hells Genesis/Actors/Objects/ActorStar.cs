using HG.Engine;
using System.IO;

namespace HG.Actors.Objects
{
    internal class ActorStar : ActorBase
    {
        private const string _assetPath = @"..\..\..\Assets\Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public ActorStar(Core core)
            : base(core)
        {
            selectedImageIndex = HGRandom.Random.Next(0, 1000) % _imageCount;
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = HGRandom.Random.Next(0, core.Display.TotalCanvasSize.Width);
            Y = HGRandom.Random.Next(0, core.Display.TotalCanvasSize.Height);

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ThrottlePercentage = HGRandom.Random.Next(8, 10) / 10.0;
            }
            else
            {
                Velocity.ThrottlePercentage = HGRandom.Random.Next(4, 8) / 10.0;
            }
        }
    }
}
