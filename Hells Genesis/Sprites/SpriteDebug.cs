using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using HG.Sprites.BaseClasses;
using System.Drawing;

namespace HG.Sprites
{
    internal class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(EngineCore core)
            : base(core)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new HgVelocity();
        }

        public SpriteDebug(EngineCore core, double x, double y)
            : base(core)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public SpriteDebug(EngineCore core, double x, double y, string imagePath)
            : base(core)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo(_core.Player.Sprite);
            base.ApplyMotion(displacementVector);
        }
    }
}
