using HG.Actors.BaseClasses;
using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorParticleTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorParticleBase => _controller.VisibleOfType<subType>();
        public List<ActorParticleBase> Visible() => _controller.VisibleOfType<ActorParticleBase>();
        public List<subType> OfType<subType>() where subType : ActorParticleBase => _controller.OfType<subType>();

        public ActorParticleTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            /*
            if (_core.Player.Actor != null)
            {
                var anchor = _core.Actors.Debugs.ByAssetTag("Anchor");
                if (anchor == null)
                {
                    _core.Actors.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = _core.Actors.Debugs.ByAssetTag("Anchor");
                }

                var pointer = _core.Actors.Debugs.ByAssetTag("Pointer");
                if (pointer == null)
                {
                    _core.Actors.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = _core.Actors.Debugs.ByAssetTag("Pointer");
                }

                double requiredAngle = _core.Player.Actor.AngleTo(anchor);
                var offset = HgMath.AngleFromPointAtDistance(new HgAngle<double>(requiredAngle), new HgPoint<double>(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = _core.Player.Actor.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(_core.Player.Actor);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorRandomParticle CreateRandomParticleAt(ActorBase actor)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRandomParticle(_core, actor.X, actor.Y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorRandomParticle CreateRandomParticleAt(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRandomParticle(_core, x, y);
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
