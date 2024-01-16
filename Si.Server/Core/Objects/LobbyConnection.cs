namespace Si.Server.Core.Objects
{
    internal class LobbyConnection
    {
        /// <summary>
        /// The unique id for the connection as established by NTDLS.ReliableMessaging.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The name that the player selected.
        /// </summary>
        public string PlayerName { get; set; } = string.Empty;

        /// <summary>
        /// The connection state for this connection is ready to start the multi-player game.
        /// </summary>
        public bool IsReadyToPlay { get; set; }

        /// <summary>
        /// The connection is waiting in the lobby.
        /// </summary>
        public bool IsWaitingInLobby { get; set; }

        /// <summary>
        /// The latest measured latency from the server to the client in milliseconds.
        /// </summary>
        public double LatencyMs { get; set; }
    }
}
