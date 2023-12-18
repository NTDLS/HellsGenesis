using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using System.Drawing;

namespace NebulaSiege.Game.Sprites
{
    internal class SpriteDebug : _SpriteShipBase
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

        public override void ApplyMotion(NsPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_core.Player.Sprite);
            base.ApplyMotion(displacementVector);
        }
    }
}
