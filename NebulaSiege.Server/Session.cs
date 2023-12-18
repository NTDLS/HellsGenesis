namespace NebulaSiege.Server
{
    public class Session
    {
        public DateTime LastSeenDatetime { get; set; }
        public Guid SessionId { get; set; }

        public Session(Guid sessionId)
        {
            SessionId = sessionId;
            LastSeenDatetime = DateTime.UtcNow;
        }
    }
}
