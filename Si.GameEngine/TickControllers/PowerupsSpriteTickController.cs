using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Powerup.BasesAndInterfaces;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared.Types.Geometry;
using System;

namespace Si.GameEngine.Controller
{
    public class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerupBase>
    {
        public PowerupsSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiReadonlyPoint displacementVector)
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
            obj.LocalLocation = new SiPoint(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
