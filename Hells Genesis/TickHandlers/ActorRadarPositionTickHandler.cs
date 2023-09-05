using HG.Actors;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickHandlers
{
    internal class ActorRadarPositionTickHandler : IUnvectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorRadarPositionIndicator => _controller.VisibleOfType<subType>();
        public List<ActorRadarPositionIndicator> Visible() => _controller.VisibleOfType<ActorRadarPositionIndicator>();
        public List<subType> OfType<subType>() where subType : ActorRadarPositionIndicator => _controller.OfType<subType>();

        public ActorRadarPositionTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
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
            lock (_controller.Collection)
            {
                var obj = new ActorRadarPositionIndicator(_core);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorRadarPositionIndicator obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                obj.Visable = false;
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
