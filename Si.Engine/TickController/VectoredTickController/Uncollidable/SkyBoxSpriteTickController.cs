using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class SkyBoxSpriteTickController : VectoredTickControllerBase<SpriteSkyBox>
    {
        public SkyBoxSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            if (displacementVector.Sum() != 0)
            {
                foreach (var skyBox in All())
                {
                    skyBox.ApplyMotion(epoch, displacementVector);
                }
            }
        }
    }
}
