using NTDLS.Semaphore;
using StrikeforceInfinity.Server.Engine.Objects;

namespace StrikeforceInfinity.Server.Engine
{
    internal class SessionManager
    {
        private readonly ServerCore _serverCore;

        readonly PessimisticSemaphore<List<Session>> _sessions = new();

        public SessionManager(ServerCore serverCore)
        {
            _serverCore = serverCore;
        }

        public void Remove(Guid connectionId)
        {
            _sessions.Use(o =>
            {
                o.RemoveAll(o => o.ConnectionId == connectionId);
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

                o.Add(sesson);

                return sesson;
            });
        }
    }
}
