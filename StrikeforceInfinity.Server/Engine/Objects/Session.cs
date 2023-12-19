namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class Session
    {
        public DateTime LastSeenDatetime { get; set; }
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// The game host that the connection is registered for, if any.
        /// </summary>
        public Guid CurrentGameHost { get; set; }

        public Session(Guid sessionId)
        {
            ConnectionId = sessionId;
            LastSeenDatetime = DateTime.UtcNow;
        }

        /// <summary>
        /// Keep track of which game host the connection was last associated with so that we can easily deregister.
        /// </summary>
        /// <param name="gameHostUID"></param>
        public void SetCurrentGameHost(Guid gameHostUID)
        {
            CurrentGameHost = gameHostUID;
        }
    }
}
