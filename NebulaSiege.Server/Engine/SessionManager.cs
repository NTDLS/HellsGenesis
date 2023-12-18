namespace NebulaSiege.Server.Engine
{
    public class SessionManager
    {
        private readonly ServerCore _core;

        readonly List<Session> _sessions = new();

        public SessionManager(ServerCore core)
        {
            _core = core;
        }

        public Session Upsert(Guid sessionId)
        {
            lock (_sessions)
            {
                var sesson = _sessions.FirstOrDefault(s => s.SessionId == sessionId);
                if (sesson != null)
                {
                    sesson.LastSeenDatetime = DateTime.UtcNow;
                    return sesson;
                }

                sesson = new Session(sessionId)
                {
                    LastSeenDatetime = DateTime.UtcNow
                };

                _sessions.Add(sesson);
                return sesson;
            }
        }
    }
}
