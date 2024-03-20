using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class SkyBoxSpriteTickController : VectoredTickControllerBase<SpriteSkyBox>
    {
        public SkyBoxSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
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
