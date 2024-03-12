using Si.Engine.Manager;
using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class PowerupSpriteTickController : VectoredTickControllerBase<SpritePowerupBase>
    {
        public PowerupSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
            }
        }

        public T Create<T>(float x, float y) where T : SpritePowerupBase
        {
            object[] param = { Engine };
            var obj = (SpritePowerupBase)Activator.CreateInstance(typeof(T), param);
            obj.Location = new SiPoint(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
