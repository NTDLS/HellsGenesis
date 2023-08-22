using AI2D.Actors.Items;
using AI2D.Engine;
using AI2D.Engine.Managers;

namespace AI2D.Actors.Factories
{
    internal class EngineActorStarFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorStarFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public ActorStar Create(double x, double y)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorStar(_core)
                {
                    X = x,
                    Y = y
                };
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorStar Create()
        {
            lock (_manager.Collection)
            {
                var obj = new ActorStar(_core);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorStar obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }
    }
}
