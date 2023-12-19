namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class RegisteredClientState
    {
        /// <summary>
        /// The unique id for the connection as established by NTDLS.ReliableMessaging.
        /// </summary>
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The client state for this connection is ready to start the multi-player game.
        /// </summary>
        public bool IsReadyToPlay { get; set; }
    }
}
