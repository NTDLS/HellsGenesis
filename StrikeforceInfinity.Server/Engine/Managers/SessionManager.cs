using NTDLS.Semaphore;
using StrikeforceInfinity.Server.Engine.Objects;

namespace StrikeforceInfinity.Server.Engine.Managers
{
    internal class SessionManager
    {
        private readonly ServerCore _serverCore;

        readonly PessimisticSemaphore<Dictionary<Guid, Session>> _sessions = new();

        public SessionManager(ServerCore serverCore)
        {
            _serverCore = serverCore;
        }

        public Session? GetByConnectionId(Guid connectionId)
        {
            return _sessions.Use(o =>
            {
                o.TryGetValue(connectionId, out var session);
                return session;
            });
        }

        public void Remove(Guid connectionId)
        {
            _sessions.Use(o =>
            {
                o.Remove(connectionId);
            });
        }

        public Session Establish(Guid connectionId)
        {
            return _sessions.Use(o =>
            {
                var sesson = new Session(connectionId)
                {
                    LastSeenDatetime = DateTime.UtcNow
                };

                o.Add(connectionId, sesson);

                return sesson;
            });
        }
    }
}
