namespace Si.Shared.Messages.Notify
{
    public class SiSpriteVector
    {
        public uint MultiplayUID { get; set; }
        public DateTime Timestamp { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }
        public double ThrottlePercentage { get; set; }
        public double BoostPercentage { get; set; }
    }
}
