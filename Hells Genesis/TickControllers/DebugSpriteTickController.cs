using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using System.Collections.Generic;
using System.Linq;

namespace HG.Controller
{
    internal class DebugSpriteTickController : IVectoredTickController
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public SpriteDebug ByAssetTag(string name) => _controller.VisibleOfType<SpriteDebug>().Where(o => o.Name == name).FirstOrDefault();

        public List<subType> VisibleOfType<subType>() where subType : SpriteDebug => _controller.VisibleOfType<subType>();
        public List<SpriteDebug> Visible() => _controller.VisibleOfType<SpriteDebug>();
        public List<subType> OfType<subType>() where subType : SpriteDebug => _controller.OfType<subType>();

        public DebugSpriteTickController(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            /*
            if (_core.Player.Sprite != null)
            {
                var anchor = _core.Sprites.Debugs.ByAssetTag("Anchor");
                if (anchor == null)
                {
                    _core.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = _core.Sprites.Debugs.ByAssetTag("Anchor");
                }

                var pointer = _core.Sprites.Debugs.ByAssetTag("Pointer");
                if (pointer == null)
                {
                    _core.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = _core.Sprites.Debugs.ByAssetTag("Pointer");
                }

                double requiredAngle = _core.Player.Sprite.AngleTo(anchor);
                var offset = HgMath.AngleFromPointAtDistance(new HgAngle(requiredAngle), new HgPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = _core.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(_core.Player.Sprite);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(displacementVector);
                debug.RenewableResources.RenewAllResources();
            }
        }

        #region Factories.

        public SpriteDebug Create(double x, double y, string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteDebug(_core, x, y)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteDebug CreateAtCenterScreen(string imagePath, string name = "")
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new SpriteDebug(_core, x, y, imagePath)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteDebug CreateAtCenterScreen(string name = "")
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new SpriteDebug(_core, x, y)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteDebug Create(double x, double y, string imagePath, string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteDebug(_core, x, y, imagePath)
                {
                    Name = name
                }; _controller.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteDebug Create(string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteDebug(_core)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteDebug obj)
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
