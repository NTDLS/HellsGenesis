using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using Si.Library.Sprite;
using System.Diagnostics;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class NebulaSpriteTickController : VectoredTickControllerBase<SpriteNebula>
    {
        public NebulaSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            if (displacementVector.Sum() != 0)
            {
                foreach (var nebula in All())
                {
                    nebula.ApplyMotion(epoch, displacementVector);
                }
            }
        }
    }
}
