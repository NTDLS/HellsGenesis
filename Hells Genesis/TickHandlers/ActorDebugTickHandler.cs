using HG.Actors.Ordinary;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickHandlers
{
    internal class ActorDebugTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public ActorDebug ByAssetTag(string name) => _controller.VisibleOfType<ActorDebug>().Where(o => o.Name == name).FirstOrDefault();

        public List<subType> VisibleOfType<subType>() where subType : ActorDebug => _controller.VisibleOfType<subType>();
        public List<ActorDebug> Visible() => _controller.VisibleOfType<ActorDebug>();
        public List<subType> OfType<subType>() where subType : ActorDebug => _controller.OfType<subType>();

        public ActorDebugTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            /*
            if (_core.Player.Actor != null)
            {
                var anchor = _core.Actors.Debugs.ByAssetTag("Anchor");
                if (anchor == null)
                {
                    _core.Actors.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = _core.Actors.Debugs.ByAssetTag("Anchor");
                }

                var pointer = _core.Actors.Debugs.ByAssetTag("Pointer");
                if (pointer == null)
                {
                    _core.Actors.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = _core.Actors.Debugs.ByAssetTag("Pointer");
                }

                double requiredAngle = _core.Player.Actor.AngleTo(anchor);
                var offset = HgMath.AngleFromPointAtDistance(new HgAngle<double>(requiredAngle), new HgPoint<double>(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = _core.Player.Actor.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(_core.Player.Actor);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public ActorDebug Create(double x, double y, string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core, x, y)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen(string imagePath, string name = "")
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y, imagePath)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug CreateAtCenterScreen(string name = "")
        {
            lock (_controller.Collection)
            {
                double x = _core.Display.TotalCanvasSize.Width / 2;
                double y = _core.Display.TotalCanvasSize.Height / 2;

                var obj = new ActorDebug(_core, x, y)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create(double x, double y, string imagePath, string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core, x, y, imagePath)
                {
                    Name = name
                }; _controller.Collection.Add(obj);
                return obj;
            }
        }

        public ActorDebug Create(string name = "")
        {
            lock (_controller.Collection)
            {
                var obj = new ActorDebug(_core)
                {
                    Name = name
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorDebug obj)
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
