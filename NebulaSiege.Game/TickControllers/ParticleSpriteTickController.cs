using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Managers;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.TickControllers.BaseClasses;
using NebulaSiege.Game.Utility;

namespace NebulaSiege.Game.Controller
{
    internal class ParticleSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(NsPoint displacementVector)
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

        public void CreateRandomShipPartParticlesAt(SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = Core.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + HgRandom.Between(-20, 20), sprite.Y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(SpriteBase sprite)
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
