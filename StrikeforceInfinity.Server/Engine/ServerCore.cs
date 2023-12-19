using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared;
using StrikeforceInfinity.Shared.Payload;
using StrikeforceInfinity.Shared.ServerMessages.Messages;
using StrikeforceInfinity.Shared.ServerMessages.Queires;

namespace StrikeforceInfinity.Server.Engine
{
    internal class ServerCore
    {
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public GameHostManager GameHost { get; private set; }

        public SiSettings Settings { get; private set; }

        MessageServer _messageServer = new();

        public ServerCore(SiSettings settings)
        {
            Settings = settings;

            Log = new LogManager(this);
            Sessions = new SessionManager(this);
            GameHost = new GameHostManager(this);
        }

        public void Start()
        {
            GameHost.Create(Guid.Empty, new SiGameHost("Canned Game Host #1", 10));
            GameHost.Create(Guid.Empty, new SiGameHost("Canned Game Host #2", 20));
            GameHost.Create(Guid.Empty, new SiGameHost("Canned Game Host #3", 30));
            GameHost.Create(Guid.Empty, new SiGameHost("Canned Game Host #4", 40));
            GameHost.Create(Guid.Empty, new SiGameHost("Canned Game Host #5", 50));

            _messageServer.Start(Settings.DataPort);

            _messageServer.OnConnected += MessageServer_OnConnected;
            _messageServer.OnDisconnected += _messageServer_OnDisconnected;
            _messageServer.OnNotificationReceived += MessageServer_OnNotificationReceived;
            _messageServer.OnQueryReceived += MessageServer_OnQueryReceived;
        }

        private IFramePayloadQueryReply MessageServer_OnQueryReceived(MessageServer server, Guid connectionId, IFramePayloadQuery payload)
        {
            if (payload is SiCreateGameHost createGameHost)
            {
                var gameHost = GameHost.Create(connectionId, createGameHost.Configuration);

                return new SiCreateGameHostReply()
                {
                    UID = gameHost.UID
                };
            }
            else if (payload is SiListGameHosts listGameHosts)
            {
                var gameHost = GameHost.GetList(connectionId);

                return new SiListGameHostsReply()
                {
                    Collection = gameHost
                };
            }
            else if (payload is SiConfigure establish)
            {
                return new SiConfigureReply()
                {
                    PlayerAbsoluteStateDelayMs = Settings.PlayerAbsoluteStateDelayMs
                };
            }
            else
            {
                throw new NotImplementedException("The server query is not implemented.");
            }
        }

        private void MessageServer_OnNotificationReceived(MessageServer server, Guid connectionId, IFramePayloadNotification payload)
        {
            if (payload is SiPlayerAbsoluteState position)
            {
                Console.WriteLine($"{position.X:n1},{position.Y:n1} -> {position.AngleDegrees:n1}");
            }
            else if (payload is SiRegisterForGameHost register)
            {
                Console.WriteLine($"ConnectionId: '{connectionId}' registered for GameHost: '{register.GameHostUID}'");
            }
            else
            {
                throw new NotImplementedException("The server notification is not implemented.");
            }
        }

        private void MessageServer_OnConnected(MessageServer server, Guid connectionId)
        {
            Sessions.Establish(connectionId);
            Console.WriteLine($"Accepted Connection: '{connectionId}'");
        }

        private void _messageServer_OnDisconnected(MessageServer server, Guid connectionId)
        {
            Sessions.Remove(connectionId);
            Console.WriteLine($"Disconnected Connection: '{connectionId}'");
        }

        public void Stop()
        {
        }
    }
}
