using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.Utility;
using System.Collections.Generic;

namespace HG.Controller
{
    internal class ParticleSpriteTickController : IVectoredTickController
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteParticleBase => _controller.VisibleOfType<subType>();
        public List<SpriteParticleBase> Visible() => _controller.VisibleOfType<SpriteParticleBase>();
        public List<SpriteParticleBase> All() => _controller.OfType<SpriteParticleBase>();
        public List<subType> OfType<subType>() where subType : SpriteParticleBase => _controller.OfType<subType>();

        public ParticleSpriteTickController(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public void CreateRandomShipPartParticlesAt(double x, double y, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = _core.Sprites.Particles.CreateRandomShipPartParticleAt(
                    x + HgRandom.Between(-20, 20), y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = _core.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + HgRandom.Between(-20, 20), sprite.Y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(SpriteBase sprite)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(_core, sprite.X, sprite.Y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(_core, x, y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteParticleBase obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
