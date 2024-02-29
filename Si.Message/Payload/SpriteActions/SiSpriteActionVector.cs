namespace Si.Library.Payload.SpriteActions
{
    public class SiSpriteActionVector : SiSpriteAction
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float AngleDegrees { get; set; }
        public float ThrottlePercentage { get; set; }
        public float BoostPercentage { get; set; }
        public float Speed { get; set; }
        public float Boost { get; set; }

        public SiSpriteActionVector(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
