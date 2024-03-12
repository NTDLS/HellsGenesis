using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    /// <summary>
    /// These are just generic bitmap sprites.
    /// </summary>
    public class GenericSpriteTickController : VectoredTickControllerBase<SpriteGenericBitmap>
    {
        public GenericSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public SpriteGenericBitmap CreateAt(SpriteBase sprite, SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGenericBitmap(Engine, sprite.Location, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteGenericBitmap Create(SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGenericBitmap(Engine, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
