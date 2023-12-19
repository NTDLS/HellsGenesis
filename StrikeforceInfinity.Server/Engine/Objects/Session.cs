namespace StrikeforceInfinity.Server.Engine.Objects
{
    internal class Session
    {
        public DateTime LastSeenDatetime { get; set; }
        public Guid ConnectionId { get; set; }

        public Session(Guid sessionId)
        {
            ConnectionId = sessionId;
            LastSeenDatetime = DateTime.UtcNow;
        }
    }
}
