using AI2D.Actors.Items;
using AI2D.Engine;
using AI2D.Engine.Managers;

namespace AI2D.Actors.Factories
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
