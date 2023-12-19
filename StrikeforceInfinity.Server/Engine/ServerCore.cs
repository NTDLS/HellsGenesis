using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Server.Engine.Managers;
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

        readonly MessageServer _messageServer = new();

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
            _messageServer.OnDisconnected += MessageServer_OnDisconnected;
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
            var session = Sessions.GetByConnectionId(connectionId);
            if (session == null)
            {
                Log.Exception($"The session was not found '{connectionId}'.");
                return;
            }

            if (payload is SiPlayerAbsoluteState position)
            {
                Log.Trace($"{position.X:n1},{position.Y:n1} -> {position.AngleDegrees:n1}");
            }
            else if (payload is SiRegisterForGameHost register)
            {
                Log.Trace($"ConnectionId: '{connectionId}' registered for GameHost: '{register.GameHostUID}'");

                var gameHost = GameHost.GetByGameHostUID(register.GameHostUID);
                if (gameHost != null)
                {
                    gameHost.Register(connectionId);
                    session.SetCurrentGameHost(register.GameHostUID);
                }
                else
                {
                    Log.Exception($"The game host was not found '{register.GameHostUID}'.");
                }
            }
            else if (payload is SiReadyToPlay)
            {
            }
            else
            {
                throw new NotImplementedException("The server notification is not implemented.");
            }
        }

        private void MessageServer_OnConnected(MessageServer server, Guid connectionId)
        {
            Sessions.Establish(connectionId);
            Log.Trace($"Accepted Connection: '{connectionId}'");
        }

        private void MessageServer_OnDisconnected(MessageServer server, Guid connectionId)
        {
            Sessions.Remove(connectionId);
            Log.Trace($"Disconnected Connection: '{connectionId}'");
        }

        public void Stop()
        {
        }
    }
}
