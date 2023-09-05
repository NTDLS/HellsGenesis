using HG.Actors;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System.Collections.Generic;
using System.Drawing;
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
            foreach (var textBlock in Visible().Where(o => o.IsPositionStatic == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorRadarPositionTextBlock CreateRadarPosition(string font, Brush color, double size, HgPoint<double> location)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorRadarPositionTextBlock(_core, font, color, size, location);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, HgPoint<double> location, bool isPositionStatic)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, HgPoint<double> location, bool isPositionStatic, string assetTag)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                obj.AssetTag = assetTag;
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
