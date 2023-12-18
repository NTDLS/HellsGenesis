using NebulaSiege.Server.Items;
using NTDLS.Semaphore;

namespace NebulaSiege.Server.Engine
{
    public class SessionManager
    {
        private readonly ServerCore _core;

        readonly PessimisticSemaphore<List<Session>> _sessions = new();

        public SessionManager(ServerCore core)
        {
            _core = core;
        }

        public Session Upsert(Guid sessionId)
        {
            return _sessions.Use(o =>
            {
                var sesson = o.FirstOrDefault(s => s.SessionId == sessionId);
                if (sesson != null)
                {
                    sesson.LastSeenDatetime = DateTime.UtcNow;
                    return sesson;
                }

                sesson = new Session(sessionId)
                {
                    LastSeenDatetime = DateTime.UtcNow
                };

                o.Add(sesson);
                return sesson;
            });
        }
    }
}
