using AI2D.Actors;
using AI2D.Actors.Enemies;
using AI2D.Actors.PowerUp;
using System;

namespace AI2D.Engine.Managers.EngineActorFactories
{
    internal class EngineActorPowerupFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorPowerupFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void Insert(PowerUpBase obj)
        {
            lock (_manager.Collection)
            {
                _manager.Collection.Add(obj);
            }
        }

        public void Delete(PowerUpBase obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        public T Create<T>() where T : PowerUpBase
        {
            lock (_manager.Collection)
            {
                object[] param = { _core };
                var obj = (PowerUpBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation(100, 1000);

                _manager.Collection.Add(obj);
                return (T)obj;
            }
        }
    }
}
