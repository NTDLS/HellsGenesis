using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.Sprites
{
    public class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            Initialize(@"Graphics\Debug.png");
            Velocity = new SiVelocity();
        }

        public SpriteDebug(GameEngineCore gameEngine, float x, float y)
            : base(gameEngine)
        {
            Initialize(@"Graphics\Debug.png");
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(GameEngineCore gameEngine, float x, float y, string imagePath)
            : base(gameEngine)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_gameEngine.Player.Sprite);
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
