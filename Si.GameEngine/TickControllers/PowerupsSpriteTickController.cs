using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.GameEngine.TickControllers
{
    public class PowerupsSpriteTickController : SpriteTickControllerBase<SpritePowerupBase>
    {
        public PowerupsSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
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
            obj.Location = new SiVector(x, y);
            SpriteManager.Add(obj);
            return (T)obj;
        }
    }
}
