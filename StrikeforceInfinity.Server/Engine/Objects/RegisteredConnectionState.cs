namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class RegisteredConnectionState
    {
        /// <summary>
        /// The unique id for the connection as established by NTDLS.ReliableMessaging.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The connection state for this connection is ready to start the multi-player game.
        /// </summary>
        public bool IsReadyToPlay { get; set; }

        /// <summary>
        /// The connection is waiting in the lobby.
        /// </summary>
        public bool IsWaitingInLobby { get; set; }
    }
}
