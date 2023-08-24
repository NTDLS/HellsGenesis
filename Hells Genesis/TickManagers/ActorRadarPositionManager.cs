using HG.Actors;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickManagers
{
    internal class ActorRadarPositionManager : IUnvectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : ActorRadarPositionIndicator => _manager.VisibleOfType<subType>();
        public List<ActorRadarPositionIndicator> VisibleOfType() => _manager.VisibleOfType<ActorRadarPositionIndicator>();
        public List<subType> OfType<subType>() where subType : ActorRadarPositionIndicator => _manager.OfType<subType>();

        public ActorRadarPositionManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<ActorRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<ActorRadarPositionTextBlock>();
                var groups = new List<List<ActorRadarPositionTextBlock>>();
                var radarTexts = _core.Actors.VisibleOfType<ActorRadarPositionTextBlock>();

                foreach (var parent in radarTexts)
                {
                    if (accountedFor.Contains(parent) == false)
                    {
                        var group = new List<ActorRadarPositionTextBlock>();
                        foreach (var child in radarTexts)
                        {
                            if (accountedFor.Contains(child) == false)
                            {
                                if (parent != child && parent.Intersects(child, new HgPoint<double>(100, 100)))
                                {
                                    group.Add(child);
                                    accountedFor.Add(child);
                                }
                            }
                        }
                        if (group.Count > 0)
                        {
                            group.Add(parent);
                            accountedFor.Add(parent);
                            groups.Add(group);
                        }
                    }
                }
                return groups;
            })();

            if (overlappingIndicators.Count > 0)
            {
                foreach (var group in overlappingIndicators)
                {
                    var min = group.Min(o => o.DistanceValue);
                    var max = group.Min(o => o.DistanceValue);

                    foreach (var member in group)
                    {
                        member.Visable = false;
                    }

                    group[0].Text = min.ToString("#,#") + "-" + max.ToString("#,#");
                    group[0].Visable = true;
                }
            }
        }

        #region Factories.

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

        #endregion
    }
}
