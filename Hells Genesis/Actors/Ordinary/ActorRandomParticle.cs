using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types;
using System.IO;
using System.Runtime;
using static HG.Engine.Constants;

namespace HG.Actors.Ordinary
{
    internal class ActorRandomParticle : ActorParticleBase
    {
        private const string _assetPath = @"Graphics\Fragments";
        private readonly int _imageCount = 31;
        private readonly int selectedImageIndex = 0;


        private double _rotationSpeed;
        private RelativeDirection _rotationDirection;
        private HgAngle<double> _travelAngle = new();

        public ActorRandomParticle(Core core, double x, double y)
            : base(core)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % _imageCount;
            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = x;
            Y = y;

            _rotationSpeed = HgRandom.RandomNumber(1, 100) / 20.0;
            _rotationDirection = HgRandom.FlipCoin() ? RelativeDirection.Left : RelativeDirection.Right;
            _travelAngle.Degrees = HgRandom.RandomNumber(0, 360);

            Velocity.ThrottlePercentage = 100;
            Velocity.MaxSpeed = HgRandom.RandomNumber(1.0, 4.0);

            _core = core;
        }

        public override void ApplyMotion(HgPoint<double> displacementVector)
        {
            X += _travelAngle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += _travelAngle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;

            if (_rotationDirection == RelativeDirection.Right)
            {
                this.Velocity.Angle.Degrees += _rotationSpeed;
            }
            else if (_rotationDirection == RelativeDirection.Left)
            {
                this.Velocity.Angle.Degrees -= _rotationSpeed;
            }

            if (_core.Display.TotalScreenBounds.IntersectsWith(Bounds) == false) //Remove off-screen stars.
            {
                QueueForDelete();
            }
        }
    }
}
