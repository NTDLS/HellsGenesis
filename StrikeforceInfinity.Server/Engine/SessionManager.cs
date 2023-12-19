using NTDLS.Semaphore;
using StrikeforceInfinity.Server.Engine.Objects;

namespace StrikeforceInfinity.Server.Engine
{
    public class SessionManager
    {
        private readonly ServerCore _core;

        readonly PessimisticSemaphore<List<Session>> _sessions = new();

        public SessionManager(ServerCore core)
        {
            _core = core;
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
