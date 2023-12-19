namespace StrikeforceInfinity.Shared.MultiplayerEvents
{
    /// <summary>
    /// Tells the server that a player has changed position, velocity and or angle.
    /// </summary>
    public class MultiplayerEventPositionChanged : MultiplayerEventBase
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double AngleDegrees { get; set; }
        public double ThrottlePercentage { get; set; }
        public double BoostPercentage { get; set; }
    }
}
