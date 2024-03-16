using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class AttachmentSpriteTickController : VectoredTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore engine, SpriteManager manager)
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

        public SpriteAttachment CreateTypeOf<T>(SpriteBase owner, string imagePath = null) where T : SpriteAttachment
        {
            var sprite = SpriteManager.CreateByType<T>();

            if (imagePath != null) sprite.SetImage(imagePath);
            sprite.ZOrder = owner.ZOrder + 1; //We want to make sure these go on top of the parent.
            sprite.OwnerUID = owner.UID;

            SpriteManager.Add(sprite);
            return sprite;
        }

        public SpriteAttachment CreateTypeOf(string typeName, SpriteBase owner, SiPoint locationRelativeToOwner)
        {
            var sprite = SpriteManager.CreateByTypeName(typeName);

            sprite.ZOrder = owner.ZOrder + 1; //We want to make sure these go on top of the parent.
            sprite.OwnerUID = owner.UID;
            sprite.LocationRelativeToOwner = locationRelativeToOwner;

            SpriteManager.Add(sprite);
            return sprite as SpriteAttachment;
        }
    }
}
