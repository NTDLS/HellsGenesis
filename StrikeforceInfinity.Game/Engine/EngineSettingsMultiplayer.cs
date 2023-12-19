namespace StrikeforceInfinity.Engine
{
    /// <summary>
    /// Some of all of these can be overridden by the server on connect.
    /// </summary>

    internal class EngineSettingsMultiplayer
    {
        /// <summary>
        /// The amount of time in milliseconds between updates of player absolute state to the server.
        /// </summary>
        public int PlayerAbsoluteStateDelayMs { get; set; }
    }
}
