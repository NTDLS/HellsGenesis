using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.TickController.SpriteTickController
{
    public class AttachmentSpriteTickController : SpriteTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var attachment in Visible())
            {
                attachment.ApplyMotion(epoch, displacementVector);
            }
        }

        public void QueueForDeletionByOwner(uint owerId)
        {
            SpriteManager.Read(o =>
            {
                SpriteManager.OfType<SpriteAttachment>()
                    .Where(o => o.OwnerUID == owerId)
                    .ToList()
                    .ForEach(c => c.QueueForDelete());
            });
        }

        public SpriteAttachment Create(SpriteBase owner, string imagePath = null)
        {
            var obj = new SpriteAttachment(Engine, imagePath)
            {
                ZOrder = owner.ZOrder + 1, //We want to make sure these go on top of the parent.
                OwnerUID = owner.UID
            };
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
