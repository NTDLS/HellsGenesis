using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using System.Drawing;

namespace StrikeforceInfinity.Game.Sprites
{
    internal class SpriteDebug : _SpriteShipBase
    {
        public SpriteDebug(EngineCore gameCore)
            : base(gameCore)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new HgVelocity();
        }

        public SpriteDebug(EngineCore gameCore, double x, double y)
            : base(gameCore)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public SpriteDebug(EngineCore gameCore, double x, double y, string imagePath)
            : base(gameCore)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_gameCore.Player.Sprite);
            base.ApplyMotion(displacementVector);
        }
    }
}
