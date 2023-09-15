using HG.Actors.BaseClasses;
using HG.Actors.Ordinary;
using HG.Controllers;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.TickHandlers.Interfaces;
using HG.Utility;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorParticleTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorParticleBase => _controller.VisibleOfType<subType>();
        public List<ActorParticleBase> Visible() => _controller.VisibleOfType<ActorParticleBase>();
        public List<subType> OfType<subType>() where subType : ActorParticleBase => _controller.OfType<subType>();

        public ActorParticleTickHandler(EngineCore core, EngineActorController manager)
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
                var obj = _core.Actors.Particles.CreateRandomShipPartParticleAt(
                    x + HgRandom.RandomNumber(-20, 20), y + HgRandom.RandomNumber(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(ActorBase actor, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = _core.Actors.Particles.CreateRandomShipPartParticleAt(
                    actor.X + HgRandom.RandomNumber(-20, 20), actor.Y + HgRandom.RandomNumber(-20, 20));
                obj.Visable = true;
            }
        }

        public ActorRandomShipPartParticle CreateRandomShipPartParticleAt(ActorBase actor)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRandomShipPartParticle(_core, actor.X, actor.Y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRandomShipPartParticle(_core, x, y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorParticleBase obj)
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
