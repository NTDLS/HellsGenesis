using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class InteractiveSpriteMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public AttachmentOrientationType OrientationType { get; set; } = AttachmentOrientationType.FixedToOwner;
        public AttachmentPositionType PositionType { get; set; } = AttachmentPositionType.FixedToOwner;

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;

        /// <summary>
        /// How much does the sprite weigh?
        /// </summary>
        public float Mass { get; set; } = 1f;
        public int Hull { get; set; } = 0;
        public int Shields { get; set; } = 0;
        public int Bounty { get; set; } = 0;
        public bool MunitionDetection { get; set; } = false;
        public bool CollisionDetection { get; set; } = false;

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        public InteractiveSpriteWeapon PrimaryWeapon { get; set; }
        public List<InteractiveSpriteAttachment> Attachments { get; set; } = new();
        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
