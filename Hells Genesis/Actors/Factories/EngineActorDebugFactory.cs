using HG.Actors.Objects;
using HG.Engine;
using HG.Engine.Managers;

namespace HG.Actors.Factories
{
    internal class EngineActorDebugFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorDebugFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public ActorDebug Create(double x, double y)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorDebug(_core, x, y);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateCenterScreen(string imagePath)
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

        public ActorDebug CreateCenterScreen()
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

    }
}
