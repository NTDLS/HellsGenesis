using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.TickControllers;
using System.Drawing;
using System.Linq;

namespace HG.Controller
{
    internal class AttachmentSpriteTickController : _SpriteTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            foreach (var attachment in Visible())
            {
                attachment.ApplyMotion(displacementVector);
            }
        }

        public void DeleteAllByOwner(uint owerId)
        {
            lock (SpriteManager.Collection)
            {
                SpriteManager.OfType<SpriteAttachment>()
                    .Where(o => o.OwnerUID == owerId)
                    .ToList()
                    .ForEach(c => c.QueueForDelete());
            }
        }

        public SpriteAttachment Create(string imagePath = null, Size? size = null, uint ownerUID = 0)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteAttachment(Core, imagePath, size)
                {
                    OwnerUID = ownerUID
                };
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
