using HG.Actors.PowerUp;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class ActorPowerupTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : PowerUpBase => _controller.VisibleOfType<subType>();
        public List<PowerUpBase> Visible() => _controller.VisibleOfType<PowerUpBase>();
        public List<subType> OfType<subType>() where subType : PowerUpBase => _controller.OfType<subType>();

        public ActorPowerupTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
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
            lock (_controller.Collection)
            {
                _controller.OfType<PowerUpBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(PowerUpBase obj)
        {
            lock (_controller.Collection)
            {
                _controller.Collection.Add(obj);
            }
        }

        public void Delete(PowerUpBase obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        public T Create<T>() where T : PowerUpBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                var obj = (PowerUpBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation(100, 1000);

                _controller.Collection.Add(obj);
                return (T)obj;
            }
        }

        public T Create<T>(double x, double y) where T : PowerUpBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                var obj = (PowerUpBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = new HgPoint<double>(x, y);
                _controller.Collection.Add(obj);
                return (T)obj;
            }
        }

        #endregion
    }
}
