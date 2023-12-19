using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.MultiplayerEvents
{
    public class MultiplayerEventPositionChanged : MultiplayerEventBase
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }

    }
}
