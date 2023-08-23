using HG.Actors.Objects;
using HG.Engine;
using HG.Engine.Managers;

namespace HG.Actors.Factories
{
    internal class EngineActorRadarPositionFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorRadarPositionFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public ActorRadarPositionIndicator Create()
        {
            lock (_manager.Collection)
            {
                var obj = new ActorRadarPositionIndicator(_core);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorRadarPositionIndicator obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                obj.Visable = false;
                _manager.Collection.Remove(obj);
            }
        }
    }
}
