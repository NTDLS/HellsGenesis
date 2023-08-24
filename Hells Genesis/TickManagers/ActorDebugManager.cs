using HG.Actors;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class ActorDebugManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : ActorDebug => _manager.VisibleOfType<subType>();
        public List<ActorDebug> VisibleOfType() => _manager.VisibleOfType<ActorDebug>();
        public List<subType> OfType<subType>() where subType : ActorDebug => _manager.OfType<subType>();

        public ActorDebugManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var debug in _manager.VisibleOfType<ActorDebug>())
            {
                debug.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorDebug Create(double x, double y)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorDebug(_core, x, y);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen(string imagePath)
        {
            lock (_manager.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y, imagePath);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen()
        {
            lock (_manager.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create(double x, double y, string imagePath)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorDebug(_core, x, y, imagePath);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create()
        {
            lock (_manager.Collection)
            {
                var obj = new ActorDebug(_core);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorDebug obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
