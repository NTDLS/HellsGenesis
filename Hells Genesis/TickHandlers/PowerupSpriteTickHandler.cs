using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites.PowerUp;
using HG.TickHandlers.Interfaces;
using System;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class PowerupSpriteTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpritePowerUpBase => _controller.VisibleOfType<subType>();
        public List<SpritePowerUpBase> Visible() => _controller.VisibleOfType<SpritePowerUpBase>();
        public List<subType> OfType<subType>() where subType : SpritePowerUpBase => _controller.OfType<subType>();

        public PowerupSpriteTickHandler(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(displacementVector);
                sprite.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<SpritePowerUpBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(SpritePowerUpBase obj)
        {
            lock (_controller.Collection)
            {
                _controller.Collection.Add(obj);
            }
        }

        public void Delete(SpritePowerUpBase obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        public T Create<T>() where T : SpritePowerUpBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                var obj = (SpritePowerUpBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation(100, 1000);

                _controller.Collection.Add(obj);
                return (T)obj;
            }
        }

        public T Create<T>(double x, double y) where T : SpritePowerUpBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                var obj = (SpritePowerUpBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = new HgPoint(x, y);
                _controller.Collection.Add(obj);
                return (T)obj;
            }
        }

        #endregion
    }
}
