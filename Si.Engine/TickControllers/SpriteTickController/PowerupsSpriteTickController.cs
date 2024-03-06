using Si.Engine.Managers;
using Si.Engine.Sprites.Powerup._Superclass;
using Si.Engine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.Engine.TickControllers.SpriteTickController
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
