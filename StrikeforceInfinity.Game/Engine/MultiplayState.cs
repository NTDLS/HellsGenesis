using System;

namespace StrikeforceInfinity.Engine
{
    internal class MultiplayState
    {
        /// <summary>
        /// The connection id of the tcp/ip connection according to the server.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The amount of time in milliseconds between updates of player absolute state to the server.
        /// </summary>
        public int PlayerAbsoluteStateDelayMs { get; set; }
    }
}
