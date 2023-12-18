using NebulaSiege.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NebulaSiege.Server.Engine
{
    public class ServerCore
    {
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }

        public NebulaSiegeSettings Settings { get; private set; }

        public ServerCore(NebulaSiegeSettings settings)
        {
            Settings = settings;

            Log = new LogManager(this);
            Sessions = new SessionManager(this);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
