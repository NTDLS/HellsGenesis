using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickHandlers
{
    internal class ActorTextBlockTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorTextBlock => _controller.VisibleOfType<subType>();
        public List<ActorTextBlock> Visible() => _controller.VisibleOfType<ActorTextBlock>();
        public List<subType> OfType<subType>() where subType : ActorTextBlock => _controller.OfType<subType>();

        public ActorTextBlockTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var textBlock in Visible().Where(o => o.IsFixedPosition == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorRadarPositionTextBlock CreateRadarPosition(TextFormat format, SolidColorBrush color, HgPoint<double> location)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRadarPositionTextBlock(_core, format, color, location);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(TextFormat format, SolidColorBrush color, HgPoint<double> location, bool isPositionStatic)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorTextBlock(_core, format, color, location, isPositionStatic);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(TextFormat format, SolidColorBrush color, HgPoint<double> location, bool isPositionStatic, string name)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorTextBlock(_core, format, color, location, isPositionStatic);
                obj.Name = name;
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorTextBlock obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
