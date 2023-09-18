using HG.Controllers;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.TickHandlers.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HG.TickHandlers
{
    internal class ActorAttachmentTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : ActorAttachment => _controller.VisibleOfType<subType>();
        public List<ActorAttachment> Visible() => _controller.VisibleOfType<ActorAttachment>();
        public List<subType> OfType<subType>() where subType : ActorAttachment => _controller.OfType<subType>();

        public ActorAttachmentTickHandler(EngineCore core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var attachment in Visible())
            {
                attachment.ApplyMotion(displacementVector);
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<ActorAttachment>().ForEach(c => c.QueueForDelete());
            }
        }

        public void DeleteAllByOwner(uint owerId)
        {
            lock (_controller.Collection)
            {
                _controller.OfType<ActorAttachment>()
                    .Where(o => o.OwnerUID == owerId)
                    .ToList()
                    .ForEach(c => c.QueueForDelete());
            }
        }

        public ActorAttachment Create(string imagePath = null, Size? size = null, uint ownerUID = 0)
        {
            lock (_controller.Collection)
            {
                var obj = new ActorAttachment(_core, imagePath, size)
                {
                    OwnerUID = ownerUID
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(ActorAttachment obj)
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
