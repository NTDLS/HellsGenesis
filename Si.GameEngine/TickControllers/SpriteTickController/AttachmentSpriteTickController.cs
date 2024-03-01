using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.GameEngine.TickControllers.SpriteTickController
{
    public class AttachmentSpriteTickController : SpriteTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
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
