using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorDebugTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorDebug => _controller.VisibleOfType<subType>();
        public List<ActorDebug> Visible() => _controller.VisibleOfType<ActorDebug>();
        public List<subType> OfType<subType>() where subType : ActorDebug => _controller.OfType<subType>();

        public ActorDebugTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var debug in Visible())
            {
                debug.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorDebug Create(double x, double y)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core, x, y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen(string imagePath)
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y, imagePath);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen()
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create(double x, double y, string imagePath)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core, x, y, imagePath);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create()
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorDebug obj)
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
