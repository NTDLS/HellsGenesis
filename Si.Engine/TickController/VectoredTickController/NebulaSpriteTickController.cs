using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

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
            foreach (var star in All())
            {
                star.ApplyMotion(epoch, displacementVector);

                //Remove stars that are too far off-screen.
                if (Engine.Display.TotalCanvasBounds.Balloon(1000).IntersectsWith(star.RenderBounds) == false)
                {
                    //star.QueueForDelete();
                }
            }
        }
    }
}