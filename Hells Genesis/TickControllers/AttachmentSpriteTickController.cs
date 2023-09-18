using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HG.Controller
{
    internal class AttachmentSpriteTickController : IVectoredTickController
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteAttachment => _controller.VisibleOfType<subType>();
        public List<SpriteAttachment> Visible() => _controller.VisibleOfType<SpriteAttachment>();
        public List<subType> OfType<subType>() where subType : SpriteAttachment => _controller.OfType<subType>();

        public AttachmentSpriteTickController(EngineCore core, EngineSpriteManager manager)
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
                _controller.OfType<SpriteAttachment>().ForEach(c => c.QueueForDelete());
            }
        }

        public void DeleteAllByOwner(uint owerId)
        {
            lock (_controller.Collection)
            {
                _controller.OfType<SpriteAttachment>()
                    .Where(o => o.OwnerUID == owerId)
                    .ToList()
                    .ForEach(c => c.QueueForDelete());
            }
        }

        public SpriteAttachment Create(string imagePath = null, Size? size = null, uint ownerUID = 0)
        {
            lock (_controller.Collection)
            {
                var obj = new SpriteAttachment(_core, imagePath, size)
                {
                    OwnerUID = ownerUID
                };
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SpriteAttachment obj)
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
