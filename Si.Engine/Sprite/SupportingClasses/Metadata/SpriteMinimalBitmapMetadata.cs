namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    internal struct SpriteMinimalBitmapMetadata
    {
        public SpriteMinimalBitmapMetadata() { }

        public string Name { get; set; }
        public string Description { get; set; }

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;
    }
}
