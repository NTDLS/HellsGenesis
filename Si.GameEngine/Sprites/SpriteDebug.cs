using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Shared.Types;
using Si.Shared.Types.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprites
{
    public class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(GameEngineCore gameEngine, double x, double y)
            : base(gameEngine)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(GameEngineCore gameEngine, double x, double y, string imagePath)
            : base(gameEngine)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_gameEngine.Player.Sprite);
            base.ApplyMotion(displacementVector);
        }
    }
}
