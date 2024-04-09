using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.GameEngine.TickController.VectoredTickController.Collidable
{
    public class DebugSpriteTickController : VectoredCollidableTickControllerBase<SpriteDebug>
    {
        public DebugSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.PerformCollisionDetection(epoch);
            }
        }
    }
}
