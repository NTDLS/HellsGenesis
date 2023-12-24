using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.Linq;

namespace Si.GameEngine.Controller
{
    public class AttachmentSpriteTickController : SpriteTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
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
                var obj = new SpriteAttachment(GameCore, imagePath, size)
                {
                    OwnerUID = ownerUID
                };
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
