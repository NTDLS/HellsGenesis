using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite
{
    public class SpriteAttachment : SpriteInteractiveBase
    {
        private SpriteInteractiveBase _rootOwner = null;
        public SiVector LocationRelativeToOwner { get; set; }

        public SpriteAttachment(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            // Since the attachement BaseLocation is relative to the top-left corner of the base sprite, we need
            // to get the position relative to the center of the base sprite image so that we can rotate around that.
            var attachmentOffset = LocationRelativeToOwner - (Owner.Size / 2.0f);

            // Apply the rotated offset to get the new attachment location relative to the base sprite center.
            Location = Owner.Location + attachmentOffset.RotatedBy(Owner.Orientation.RadiansSigned);

            //Make sure the attachment faces forwards.
            Orientation = Owner.Orientation;

            base.ApplyMotion(epoch, displacementVector);
        }

        /// <summary>
        /// Gets and caches the root owner of this attachement.
        /// </summary>
        /// <returns></returns>
        public SpriteInteractiveBase Owner
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
