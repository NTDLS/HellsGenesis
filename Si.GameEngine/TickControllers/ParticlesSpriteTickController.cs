using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.Controller
{
    public class ParticlesSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
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
                var obj = new SpriteRandomShipPartParticle(GameCore, sprite.X, sprite.Y);
                SpriteManager.Insert(obj);
                return obj;
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
                var obj = new SpriteRandomShipPartParticle(GameCore, x, y);
                SpriteManager.Insert(obj);
                return obj;
        }
    }
}
