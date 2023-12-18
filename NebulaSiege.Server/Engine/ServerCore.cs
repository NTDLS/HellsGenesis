using NebulaSiege.Client.Payloads;
using NebulaSiege.Shared;

namespace NebulaSiege.Server.Engine
{
    public class ServerCore
    {
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public GameHostManager GameHost { get; private set; }

        public NebulaSiegeSettings Settings { get; private set; }

        public ServerCore(NebulaSiegeSettings settings)
        {
            Settings = settings;

            Log = new LogManager(this);
            Sessions = new SessionManager(this);
            GameHost = new GameHostManager(this);
        }

        public void Start()
        {
            GameHost.Create(Guid.Empty, new NsGameHost("Canned Game Host #1", 10));
            GameHost.Create(Guid.Empty, new NsGameHost("Canned Game Host #2", 20));
            GameHost.Create(Guid.Empty, new NsGameHost("Canned Game Host #3", 30));
            GameHost.Create(Guid.Empty, new NsGameHost("Canned Game Host #4", 40));
            GameHost.Create(Guid.Empty, new NsGameHost("Canned Game Host #5", 50));
        }

        public void Stop()
        {
        }
    }
}
