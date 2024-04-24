using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    internal class SpriteAnimationMetadata
    {
        public SpriteAnimationMetadata() { }

        public string Name { get; set; }
        public string Description { get; set; }

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FramesPerSecond { get; set; }

        public SiAnimationPlayMode PlayMode { get; set; }
    }
}
