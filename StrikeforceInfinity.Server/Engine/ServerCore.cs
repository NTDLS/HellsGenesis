using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Server.Engine.Managers;
using StrikeforceInfinity.Shared;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Shared.Messages.Query;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Server.Engine
{
    internal class ServerCore
    {
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public LobbyManager Lobbies { get; private set; }
        public SiSettings Settings { get; private set; }

        readonly MessageServer _messageServer = new();

        public ServerCore(SiSettings settings)
        {
            Settings = settings;

            Log = new LogManager(this);
            Sessions = new SessionManager(this);
            Lobbies = new LobbyManager(this);
        }

        public void Start()
        {
            _messageServer.Start(Settings.DataPort);

            _messageServer.OnConnected += MessageServer_OnConnected;
            _messageServer.OnDisconnected += MessageServer_OnDisconnected;
            _messageServer.OnNotificationReceived += MessageServer_OnNotificationReceived;
            _messageServer.OnQueryReceived += MessageServer_OnQueryReceived;
        }

        private IFramePayloadQueryReply MessageServer_OnQueryReceived(MessageServer server, Guid connectionId, IFramePayloadQuery payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiGetLobbyInfo getLobbyInfo)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' getting lobby info for '{getLobbyInfo.LobyUID}'.");

                var lobby = Lobbies.GetByLobbyUID(getLobbyInfo.LobyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{getLobbyInfo.LobyUID}'.");
                    return new SiGetLobbyInfoReply();
                }

                return new SiGetLobbyInfoReply()
                {
                    Info = new SiLobbyInfo
                    {
                        Name = lobby.Name,
                        WaitingCount = lobby.ConnectionsWaitingInLobbyCount()
                    }
                };
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiCreateLobby createLobby)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' creating lobby '{createLobby.Configuration.Name}'.");

                var lobby = Lobbies.Create(connectionId, createLobby.Configuration);

                return new SiCreateLobbyReply()
                {
                    LobbyUID = lobby.UID
                };
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiListLobbies listLobbies)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' requested lobby list.");

                var lobbies = Lobbies.GetList(connectionId);

                return new SiListLobbiesReply()
                {
                    Collection = lobbies
                };
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiConfigure establish)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' requested configuration.");

                return new SiConfigureReply()
                {
                    PlayerAbsoluteStateDelayMs = Settings.PlayerAbsoluteStateDelayMs
                };
            }
            //------------------------------------------------------------------------------------------------------------------------------
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

            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSpriteVector spriteVector)
            {
                Log.Trace($"{spriteVector.X:n1},{spriteVector.Y:n1} -> {spriteVector.AngleDegrees:n1}");

                var lobby = Lobbies.GetByLobbyUID(session.CurrentLobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{session.CurrentLobbyUID}'.");
                    return;
                }

                //Broadcast the absolute state to all connections except for the one that sent it.
                var registeredConnectionIds = lobby.RegisteredConnections();
                registeredConnectionIds.Remove(connectionId);
                foreach (var registeredConnectionId in registeredConnectionIds)
                {
                    _messageServer.Notify(registeredConnectionId, spriteVector);
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiRegisterToLobby register)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' registered for lobby: '{register.LobbyUID}'");

                var lobby = Lobbies.GetByLobbyUID(register.LobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{register.LobbyUID}'.");
                    return;
                }

                lobby.Register(connectionId);
                session.SetCurrentLobby(register.LobbyUID);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiWaitingInLobby)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' is waiting in the lobby.");

                var lobby = Lobbies.GetByLobbyUID(session.CurrentLobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{session.CurrentLobbyUID}'.");
                    return;
                }

                lobby.FlagConnectionAsWaitingInLobby(connectionId);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiSituationLayout layoutDirective)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' issued a new layout.");

                var lobby = Lobbies.GetByLobbyUID(session.CurrentLobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{session.CurrentLobbyUID}'.");
                    return;
                }

                //The owner of the lobby send a new layout, send it to all of the connections.
                var registeredConnectionIds = lobby.RegisteredConnections();
                foreach (var registeredConnectionId in registeredConnectionIds)
                {
                    _messageServer.Notify(registeredConnectionId, layoutDirective);
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiReadyToPlay)
            {
                Log.Verbose($"ConnectionId: '{connectionId}' is ready to play.");

                var lobby = Lobbies.GetByLobbyUID(session.CurrentLobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{session.CurrentLobbyUID}'.");
                    return;
                }

                lobby.FlagConnectionAsReadyToPlay(connectionId);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else
            {
                throw new NotImplementedException("The server notification is not implemented.");
            }
        }

        private void MessageServer_OnConnected(MessageServer server, Guid connectionId)
        {
            Sessions.Establish(connectionId);
            Log.Verbose($"Accepted Connection: '{connectionId}'");
        }

        private void MessageServer_OnDisconnected(MessageServer server, Guid connectionId)
        {
            Sessions.Remove(connectionId);
            Log.Verbose($"Disconnected Connection: '{connectionId}'");
        }

        public void Stop()
        {
        }
    }
}
