using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite
{
    public class SpriteDebug : SpriteInteractiveShipBase
    {
        public SpriteDebug(EngineCore engine)
            : base(engine)
        {
            Initialize(@"Sprites\Debug.png");
        }

        public SpriteDebug(EngineCore engine, float x, float y)
            : base(engine)
        {
            Initialize(@"Sprites\Debug.png");
            X = x;
            Y = y;
        }

        public SpriteDebug(EngineCore engine, float x, float y, string imagePath)
            : base(engine)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
        }

        private void Initialize(string imagePath)
        {
            SetImageAndLoadMetadata(imagePath);
            SetHullHealth(100000);
            Speed = 1.5f;
            Throttle = 0.05f;
            RecalculateMovementVector();
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            PointingAngle.Degrees = AngleToInUnsignedDegrees(_engine.Player.Sprite);
            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
