using static Si.Shared.SiConstants;

namespace Si.MultiplayClient
{
    public class MultiplayState
    {
        /// <summary>
        /// The connection id of the tcp/ip connection according to the server.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The amount of time in milliseconds between updates of player absolute state to the server.
        /// </summary>
        public int PlayerAbsoluteStateDelayMs { get; set; }

        public SiPlayMode PlayMode { get; set; }
        public Guid LobbyUID { get; set; } = Guid.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }
}
