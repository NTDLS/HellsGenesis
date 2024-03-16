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
            /*
            if (displacementVector.Sum() != 0)
            {
                var screen = new SiPoint(Engine.Display.TotalCanvasSize.Width, Engine.Display.TotalCanvasSize.Height);

                var allNebulas = All();

                foreach (var nebula in allNebulas)
                {
                    if (Engine.Display.TotalCanvasBounds.IntersectsWith(nebula.RenderBounds))
                    {
                        nebula.ApplyMotion(epoch, displacementVector);
                    }
                }

                var sprite = Engine.Player.Sprite.FindFirstCollisionAlongDistanceVector(allNebulas, screen.Sum());
                if (sprite == null)
                {
                    //We might not see a nebula in the line of sight, but there might be one 2px above or below.
                    //Maybe we need to add a cone of obverbvance to FindFirstCollisionAlongDistanceVector?
                    //Or maybe just widen the projected detection "beam"?

                    var newNebula = Engine.Sprites.Nebulas.AddAtCenterScreen();
                    newNebula.Location += (displacementVector.Normalize()) * ((screen.Sum() / 2.0f) + newNebula.Size.Width);

                    Debug.WriteLine($"Added Nebula at: {newNebula.Location}");

                }
            }
            */
        }
    }
}
