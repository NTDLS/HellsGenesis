using HG.Actors.PowerUp;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class ActorPowerupManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : PowerUpBase => _manager.VisibleOfType<subType>();
        public List<PowerUpBase> Visible() => _manager.VisibleOfType<PowerUpBase>();
        public List<subType> OfType<subType>() where subType : PowerUpBase => _manager.OfType<subType>();

        public ActorPowerupManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var powerUp in _core.Actors.VisibleOfType<PowerUpBase>())
            {
                HgConversion.DynamicCast(powerUp, powerUp.GetType()).ApplyIntelligence(displacementVector);
                powerUp.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

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
                obj.Location = new HgPoint<double>(x, y);
                _manager.Collection.Add(obj);
                return (T)obj;
            }
        }

        #endregion
    }
}
