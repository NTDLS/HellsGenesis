using Si.Engine;
using Si.GameEngine.Manager;
using Si.GameEngine.Sprite.PowerUp._Superclass;
using Si.GameEngine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.GameEngine.TickController.SpriteTickController
{
    public class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerupBase>
    {
        public PowerupsSpriteTickController(EngineCore engine, EngineSpriteManager manager)
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
            object[] param = { GameEngine };
            var obj = (SpritePowerupBase)Activator.CreateInstance(typeof(T), param);
            obj.Location = new SiPoint(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
