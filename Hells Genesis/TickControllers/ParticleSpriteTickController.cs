using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.TickControllers;
using HG.Utility;

namespace HG.Controller
{
    internal class ParticleSpriteTickController : _SpriteTickControllerBase<_SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
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
                var obj = Core.Sprites.Particles.CreateRandomShipPartParticleAt(
                    x + HgRandom.Between(-20, 20), y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(_SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = Core.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + HgRandom.Between(-20, 20), sprite.Y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(_SpriteBase sprite)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(Core, sprite.X, sprite.Y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(Core, x, y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
