using Si.Engine;
using Si.GameEngine.Manager;
using Si.GameEngine.Sprite;
using Si.GameEngine.Sprite._Superclass;
using Si.GameEngine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.GameEngine.TickController.SpriteTickController
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

        public void DeleteAllByOwner(uint owerId)
        {
            SpriteManager.Use(o =>
            {
                SpriteManager.OfType<SpriteAttachment>()
                    .Where(o => o.OwnerUID == owerId)
                    .ToList()
                    .ForEach(c => c.QueueForDelete());
            });
        }

        public SpriteAttachment Create(SpriteBase owner, string imagePath = null)
        {
            var obj = new SpriteAttachment(GameEngine, imagePath)
            {
                ZOrder = owner.ZOrder + 1, //We want to make sure these go on top of the stack.
                OwnerUID = owner.UID
            };
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
