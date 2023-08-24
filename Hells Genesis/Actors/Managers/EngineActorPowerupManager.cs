using HG.Actors.Objects.PowerUp;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;
using System;

namespace HG.Actors.Factories
{
    internal class EngineActorPowerupManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorPowerupManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HGPoint<double> appliedOffset)
        {
            foreach (var powerUp in _core.Actors.VisibleOfType<PowerUpBase>())
            {
                HGConversion.DynamicCast(powerUp, powerUp.GetType()).ApplyIntelligence(appliedOffset);
                powerUp.ApplyMotion(appliedOffset);
            }
        }

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<PowerUpBase>().ForEach(c => c.QueueForDelete());
            }
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

        public T Create<T>(double x, double y) where T : PowerUpBase
        {
            lock (_manager.Collection)
            {
                object[] param = { _core };
                var obj = (PowerUpBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = new Types.HGPoint<double>(x, y);
                _manager.Collection.Add(obj);
                return (T)obj;
            }
        }
    }
}
