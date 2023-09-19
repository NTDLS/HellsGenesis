using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites.PowerUp;
using HG.TickControllers;
using System;

namespace HG.Controller
{
    internal class PowerupSpriteTickController : _SpriteTickControllerBase<_SpritePowerUpBase>
    {
        public PowerupSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(displacementVector);
                sprite.ApplyMotion(displacementVector);
            }
        }

        public T Create<T>(double x, double y) where T : _SpritePowerUpBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { Core };
                var obj = (_SpritePowerUpBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = new HgPoint(x, y);
                SpriteManager.Collection.Add(obj);
                return (T)obj;
            }
        }
    }
}
