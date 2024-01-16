using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Core.TickControllers
{
    public class ParticlesSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
    {
        public ParticlesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
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
                var obj = GameEngine.Sprites.Particles.CreateRandomShipPartParticleAt(
                    x + SiRandom.Between(-20, 20), y + SiRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameEngine.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + SiRandom.Between(-20, 20), sprite.Y + SiRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(SpriteBase sprite)
        {
            var obj = new SpriteRandomShipPartParticle(GameEngine, sprite.X, sprite.Y);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            var obj = new SpriteRandomShipPartParticle(GameEngine, x, y);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
