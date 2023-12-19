namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class Session
    {
        public DateTime LastSeenDatetime { get; set; }
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The lobby that the connection is registered for, if any.
        /// </summary>
        public Guid CurrentLobbyUID { get; set; }

        public Session(Guid sessionId)
        {
            ConnectionId = sessionId;
            LastSeenDatetime = DateTime.UtcNow;
        }

        /// <summary>
        /// Keep track of which lobby the connection was last associated with so that we can easily deregister.
        /// </summary>
        /// <param name="gameLobbyUID"></param>
        public void SetCurrentLobby(Guid lobbyUID)
        {
            CurrentLobbyUID = lobbyUID;
        }
    }
}
