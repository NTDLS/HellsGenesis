using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites.PowerUp.BasesAndInterfaces;
using Si.Game.TickControllers.BasesAndInterfaces;
using System;

namespace Si.Game.Controller
{
    internal class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerUpBase>
    {
        public PowerupsSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(displacementVector);
                sprite.ApplyMotion(displacementVector);
            }
        }

        public T Create<T>(double x, double y) where T : SpritePowerUpBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                var obj = (SpritePowerUpBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = new SiPoint(x, y);
                SpriteManager.Collection.Add(obj);
                return (T)obj;
            }
        }
    }
}
