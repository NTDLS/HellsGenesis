using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System.IO;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteRandomShipPartParticle : SpriteParticleBase
    {
        private const string _assetPath = @"Graphics\Fragments";
        private readonly int _imageCount = 1488;
        private readonly int selectedImageIndex = 0;


        private double _rotationSpeed;
        private SiRelativeDirection _rotationDirection;
        private SiAngle _travelAngle = new();

        public SpriteRandomShipPartParticle(Engine gameCore, double x, double y)
            : base(gameCore)
        {
            selectedImageIndex = SiRandom.Generator.Next(0, 1000) % _imageCount;

            Initialize(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            X = x;
            Y = y;

            _rotationSpeed = SiRandom.Between(1, 100) / 20.0;
            _rotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
            _travelAngle.Degrees = SiRandom.Between(0, 360);

            Velocity.ThrottlePercentage = 100;
            Velocity.MaxSpeed = SiRandom.Between(1.0, 4.0);

            _gameCore = gameCore;
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            X += _travelAngle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);
            Y += _travelAngle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage);

            if (_rotationDirection == SiRelativeDirection.Right)
            {
                Velocity.Angle.Degrees += _rotationSpeed;
            }
            else if (_rotationDirection == SiRelativeDirection.Left)
            {
                Velocity.Angle.Degrees -= _rotationSpeed;
            }

            if (_gameCore.Display.TotalCanvasBounds.IntersectsWith(Bounds) == false) //Remove off-screen stars.
            {
                QueueForDelete();
            }
        }
    }
}
