using Si.Game.Engine;
using Si.Game.Managers;
using Si.Game.Sprites;
using Si.Game.TickControllers.BasesAndInterfaces;
using Si.Game.Utility;
using Si.Shared;
using Si.Shared.Types.Geometry;

namespace Si.Game.Controller
{
    internal class ParticlesSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
    {
        public ParticlesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(displacementVector);
            }
        }

        public void CreateRandomShipPartParticlesAt(double x, double y, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameCore.Sprites.Particles.CreateRandomShipPartParticleAt(
                    x + SiRandom.Between(-20, 20), y + SiRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameCore.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + SiRandom.Between(-20, 20), sprite.Y + SiRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(SpriteBase sprite)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(GameCore, sprite.X, sprite.Y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(GameCore, x, y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
