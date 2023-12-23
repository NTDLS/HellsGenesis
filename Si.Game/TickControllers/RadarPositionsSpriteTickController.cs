using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites;
using Si.Game.TickControllers.BasesAndInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Game.Controller
{
    internal class RadarPositionsSpriteTickController : UnvectoredTickControllerBase<SpriteRadarPositionTextBlock>
    {
        private readonly EngineSpriteManager _manager;

        public RadarPositionsSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore)
        {
            _manager = manager;
        }

        public override void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<SpriteRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<SpriteRadarPositionTextBlock>();
                var groups = new List<List<SpriteRadarPositionTextBlock>>();
                var radarTexts = GameCore.Sprites.VisibleOfType<SpriteRadarPositionTextBlock>();

                foreach (var parent in radarTexts)
                {
                    if (accountedFor.Contains(parent) == false)
                    {
                        var group = new List<SpriteRadarPositionTextBlock>();
                        foreach (var child in radarTexts)
                        {
                            if (accountedFor.Contains(child) == false)
                            {
                                if (parent != child && parent.Intersects(child, new SiPoint(100, 100)))
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

        public SpriteRadarPositionIndicator Create()
        {
            lock (_manager.Collection)
            {
                var obj = new SpriteRadarPositionIndicator(GameCore);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteRadarPositionIndicator obj)
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
