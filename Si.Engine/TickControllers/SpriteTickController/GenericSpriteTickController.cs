using Si.Engine.Managers;
using Si.Engine.Sprites;
using Si.Engine.Sprites._Superclass;
using Si.Engine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.TickControllers.SpriteTickController
{
    /// <summary>
    /// These are just generic bitmap sprites.
    /// </summary>
    public class GenericSpriteTickController : SpriteTickControllerBase<SpriteGeneric>
    {
        public GenericSpriteTickController(EngineCore engine, EngineSpriteManager manager)
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

        public SpriteGeneric CreateAt(SpriteBase sprite, SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, sprite.Location, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteGeneric Create(SharpDX.Direct2D1.Bitmap bitmap)
        {
            var obj = new SpriteGeneric(GameEngine, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
