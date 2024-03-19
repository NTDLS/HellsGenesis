using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite
{
    public class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(EngineCore engine)
            : base(engine)
        {
            SetImage(@"Sprites\Debug.png");
            Velocity = new SiVelocity();

            SetHullHealth(100000);
        }

        public SpriteDebug(EngineCore engine, float x, float y)
            : base(engine)
        {
            SetImage(@"Sprites\Debug.png");
            X = x;
            Y = y;
            Velocity = new SiVelocity();

            SetHullHealth(100000);
        }

        public SpriteDebug(EngineCore engine, float x, float y, string imagePath)
            : base(engine)
        {
            SetImage(imagePath);
            X = x;
            Y = y;
            Velocity = new SiVelocity();

            SetHullHealth(100000);
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Velocity.ForwardAngle.Degrees = AngleTo360(_engine.Player.Sprite);
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
