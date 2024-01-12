using NTDLS.Semaphore;
using Si.Server.Engine.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Si.Server.Engine.Managers
{
    internal class SessionManager
    {
        private readonly ServerCore _serverCore;
        private readonly PessimisticSemaphore<Dictionary<Guid, Session>> _sessions = new();

        public SessionManager(ServerCore serverCore)
        {
            _serverCore = serverCore;
        }

        public bool TryGetByConnectionId(Guid connectionId, [NotNullWhen(true)] out Session? outSession)
        {
            var result = _sessions.Use(o =>
            {
                o.TryGetValue(connectionId, out var session);
                return session;
            });

            outSession = result;
            return outSession != null;
        }

        public void Remove(Guid connectionId)
        {
            _sessions.Use(o =>
            {
                o.Remove(connectionId);
            });
        }

        public Session Establish(Guid connectionId, IPAddress ipAdress)
        {
            return _sessions.Use(o =>
            {
                var sesson = new Session(connectionId, ipAdress)
                {
                    LastSeenDatetime = DateTime.UtcNow,
                };

                o.Add(connectionId, sesson);

                return sesson;
            });
        }
    }
}
