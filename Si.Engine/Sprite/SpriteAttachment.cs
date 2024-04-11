using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite
{
    public class SpriteAttachment : SpriteInteractiveBase
    {
        private SpriteInteractiveBase _rootOwner = null;
        public SiVector LocationRelativeToOwner { get; set; }

        public SpriteAttachment(EngineCore engine)
            : base(engine)
        {
        }

        public SpriteAttachment(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (IsDeadOrExploded) return;

            // Since the attachement BaseLocation is relative to the top-left corner of the base sprite, we need
            // to get the position relative to the center of the base sprite image so that we can rotate around that.
            var attachmentOffset = LocationRelativeToOwner - (OwnerSprite.Size / 2.0f);

            // Apply the rotated offset to get the new attachment location relative to the base sprite center.
            Location = OwnerSprite.Location + attachmentOffset.RotatedBy(OwnerSprite.Orientation.RadiansSigned);

            //Make sure the attachment faces forwards.
            Orientation = OwnerSprite.Orientation;

            base.ApplyMotion(epoch, displacementVector);
        }

        /// <summary>
        /// Gets and caches the root owner of this attachement.
        /// </summary>
        /// <returns></returns>
        public SpriteInteractiveBase OwnerSprite
        {
            get
            {
                if (_rootOwner == null)
                {
                    _rootOwner = this;

                    do
                    {
                        _rootOwner = _engine.Sprites.GetSpriteByOwner<SpriteInteractiveBase>(_rootOwner.OwnerUID);

                    } while (_rootOwner.OwnerUID != 0);
                }
                return _rootOwner;
            }
        }
    }
}
