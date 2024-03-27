using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite
{
    public class SpriteDebug : SpriteShipBase
    {
        public SpriteDebug(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Debug.png");
            SetHullHealth(100000);
            ThrottlePercentage = 0;
        }

        public SpriteDebug(EngineCore engine, float x, float y)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Debug.png");
            X = x;
            Y = y;
            SetHullHealth(100000);
            ThrottlePercentage = 0;
        }

        public SpriteDebug(EngineCore engine, float x, float y, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
            X = x;
            Y = y;
            SetHullHealth(100000);
            ThrottlePercentage = 0;
        }

        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Direction.Degrees = AngleTo360(_engine.Player.Sprite);
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
