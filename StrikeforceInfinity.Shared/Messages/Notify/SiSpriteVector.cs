using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server where exactly a sprite is, which direction they are facing and how fast they are going.
    /// </summary>
    public class SiSpriteVector : IFramePayloadNotification
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
