using HG.Actors;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HG.TickManagers
{
    internal class ActorTextBlockManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : ActorTextBlock => _manager.VisibleOfType<subType>();
        public List<ActorTextBlock> Visible() => _manager.VisibleOfType<ActorTextBlock>();
        public List<subType> OfType<subType>() where subType : ActorTextBlock => _manager.OfType<subType>();

        public ActorTextBlockManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var textBlock in VisibleOfType<ActorTextBlock>().Where(o => o.IsPositionStatic == false))
            {
                textBlock.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorRadarPositionTextBlock CreateRadarPosition(string font, Brush color, double size, HgPoint<double> location)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorRadarPositionTextBlock(_core, font, color, size, location);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, HgPoint<double> location, bool isPositionStatic)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock Create(string font, Brush color, double size, HgPoint<double> location, bool isPositionStatic, string assetTag)
        {
            lock (_manager.Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                obj.AssetTag = assetTag;
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorTextBlock obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
