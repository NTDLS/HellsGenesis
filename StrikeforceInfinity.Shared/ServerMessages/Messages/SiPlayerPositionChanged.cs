using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.ServerMessages.Messages
{
    /// <summary>
    /// Tells the server that a player has changed position, velocity and or angle.
    /// </summary>
    public class SiPlayerPositionChanged : IFramePayloadNotification
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }
        public double ThrottlePercentage { get; set; }
        public double BoostPercentage { get; set; }
    }
}
