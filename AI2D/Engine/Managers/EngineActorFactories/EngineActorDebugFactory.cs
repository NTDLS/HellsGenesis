using AI2D.Actors;

namespace AI2D.Engine.Managers.EngineActorFactories
{
    public class EngineActorDebugFactory
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
