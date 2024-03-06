using Si.Engine.Managers;
using Si.Engine.Sprites;
using Si.Engine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Engine.TickControllers.SpriteTickController
{
    public class RadarPositionsSpriteTickController : UnvectoredTickControllerBase<SpriteRadarPositionTextBlock>
    {
        private readonly EngineSpriteManager _manager;

        public RadarPositionsSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine)
        {
            _manager = manager;
        }

        public override void ExecuteWorldClockTick()
        {
            var overlappingIndicators = new Func<List<List<SpriteRadarPositionTextBlock>>>(() =>
            {
                var accountedFor = new HashSet<SpriteRadarPositionTextBlock>();
                var groups = new List<List<SpriteRadarPositionTextBlock>>();
                var radarTexts = GameEngine.Sprites.VisibleOfType<SpriteRadarPositionTextBlock>();

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
            var obj = new SpriteRadarPositionIndicator(GameEngine);
            _manager.Add(obj);
            return obj;
        }

        public void Delete(SpriteRadarPositionIndicator obj)
        {
            obj.Visable = false;
            _manager.Delete(obj);
        }

        #endregion
    }
}
