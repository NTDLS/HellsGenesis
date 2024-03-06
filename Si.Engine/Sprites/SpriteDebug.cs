using Si.Engine.Sprites._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprites
{
    public class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(EngineCore engine)
            : base(engine)
        {
            Initialize(@"Graphics\Debug.png");
            Velocity = new SiVelocity();
        }

        public SpriteDebug(EngineCore engine, float x, float y)
            : base(engine)
        {
            Initialize(@"Graphics\Debug.png");
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public SpriteDebug(EngineCore engine, float x, float y, string imagePath)
            : base(engine)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new SiVelocity();
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Velocity.Angle.Degrees = AngleTo360(_engine.Player.Sprite);
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
