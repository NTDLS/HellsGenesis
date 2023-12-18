using NebulaSiege.Client.Payloads;
using NebulaSiege.Shared;
using NebulaSiege.Shared.MultiplayerEvents;
using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using System.Diagnostics;

namespace NebulaSiege.Server.Engine
{
    public class ServerCore
    {
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public GameHostManager GameHost { get; private set; }

        public NebulaSiegeSettings Settings { get; private set; }

        MessageServer _messageServer = new();

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

            _messageServer.Start(Settings.DataPort);

            _messageServer.OnConnected += MessageServer_OnConnected;
            _messageServer.OnNotificationReceived += MessageServer_OnNotificationReceived;
        }

        private void MessageServer_OnNotificationReceived(MessageServer server, Guid connectionId, IFramePayloadNotification payload)
        {
            if (payload is MultiplayerEventPositionChanged  position)
            {
                Debug.WriteLine($"{position.X:n1},{position.Y:n1} -> {position.AngleDegrees:n1}");
            }
        }

        private void MessageServer_OnConnected(MessageServer server, Guid connectionId)
        {
        }

        public void Stop()
        {
        }
    }
}
