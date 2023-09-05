using HG.Engine;
using HG.Types;
using System.IO;

namespace HG.Actors
{
    internal class ActorStar : ActorBase
    {
        private const string _assetPath = @"Graphics\Star\";
        private readonly int _imageCount = 5;
        private readonly int selectedImageIndex = 0;

        public ActorStar(Core core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % _imageCount;
            InitializeGenericBasic(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = HgRandom.Random.Next(0, core.Display.TotalCanvasSize.Width);
            Y = HgRandom.Random.Next(0, core.Display.TotalCanvasSize.Height);

            if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            {
                Velocity.ThrottlePercentage = HgRandom.Random.Next(8, 10) / 10.0;
            }
            else
            {
                Velocity.ThrottlePercentage = HgRandom.Random.Next(4, 8) / 10.0;
            }
        }

        public new void ApplyMotion(HgPoint<double> displacementVector)
        {
            X -= displacementVector.X * Velocity.ThrottlePercentage;
            Y -= displacementVector.Y * Velocity.ThrottlePercentage;

            if (_core.Display.TotalScreenBounds.IntersectsWith(Bounds) == false) //Remove off-screen stars.
            {
                QueueForDelete();
            }
        }
    }
}
