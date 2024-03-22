using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.TickController.VectoredTickController.Collidable
{
    /// <summary>
    /// These are just generic bitmap sprites.
    /// </summary>
    public class GenericSpriteTickController : VectoredCollidableTickControllerBase<SpriteGenericBitmap>
    {
        public GenericSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector, SpriteInteractiveBase[] collidables)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.PerformCollisionDetection(collidables);
            }
        }
    }
}
