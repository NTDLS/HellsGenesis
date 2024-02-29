using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class AttachmentSpriteTickController : SpriteTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(double epoch, SiVector displacementVector)
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

        public SpriteAttachment Create(string imagePath = null, uint ownerUID = 0)
        {
            var obj = new SpriteAttachment(GameEngine, imagePath)
            {
                OwnerUID = ownerUID
            };
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
