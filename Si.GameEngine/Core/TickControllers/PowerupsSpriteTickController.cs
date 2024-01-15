using Si.GameEngine.Core;
using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.Shared.Types.Geometry;
using System;

namespace Si.GameEngine.Core.TickControllers
{
    public class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerupBase>
    {
        public PowerupsSpriteTickController(Engine gameCore, EngineSpriteManager manager)
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

        public T Create<T>(double x, double y) where T : SpritePowerupBase
        {
            object[] param = { GameCore };
            var obj = (SpritePowerupBase)Activator.CreateInstance(typeof(T), param);
            obj.Location = new SiPoint(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
