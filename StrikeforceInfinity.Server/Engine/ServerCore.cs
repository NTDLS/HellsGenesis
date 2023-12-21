using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using StrikeforceInfinity.Server.Engine.Managers;
using StrikeforceInfinity.Shared;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Shared.Messages.Query;
using StrikeforceInfinity.Shared.Payload;
using System.Net;
using System.Net.Sockets;

namespace StrikeforceInfinity.Server.Engine
{
    internal class ServerCore
    {
        /// <summary>
        /// A list of IPaddress endpoints for all connections, these are used for UDP sending.
        /// </summary>
        private Dictionary<Guid, IPEndPoint> _activeEndpoints = new();
        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public LobbyManager Lobbies { get; private set; }
        public SiSettings Settings { get; private set; }

        readonly MessageServer _messageServer = new();
        readonly UdpMessageManager _udpManager;

        public ServerCore(SiSettings settings)
        {
            Settings = settings;

            _udpManager = new UdpMessageManager(Settings.DataPort, UdpMessageManager_ProcessNotificationCallback);

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

            //LetsTryUDP();
        }

        private void UdpMessageManager_ProcessNotificationCallback(IUDPPayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSpriteVectors spriteVectors)
            {
                var session = Sessions.GetByConnectionId(spriteVectors.ConnectionId);
                if (session == null)
                {
                    Log.Exception($"The session was not found '{spriteVectors.ConnectionId}'.");
                    return;
                }

                //Log.Trace($"{spriteVector.X:n1},{spriteVector.Y:n1} -> {spriteVector.AngleDegrees:n1}");

                var lobby = Lobbies.GetByLobbyUID(session.CurrentLobbyUID);
                if (lobby == null)
                {
                    Log.Exception($"The lobby was not found '{session.CurrentLobbyUID}'.");
                    return;
                }

                //Broadcast the absolute state to all connections except for the one that sent it.
                var registeredConnectionIds = lobby.RegisteredConnections();
                registeredConnectionIds.Remove(spriteVectors.ConnectionId);
                foreach (var registeredConnectionId in registeredConnectionIds)
                {
                    if (_activeEndpoints.TryGetValue(registeredConnectionId, out var endpoint))
                    {
                        try
                        {
                            _udpManager.WriteMessage(endpoint, spriteVectors);
                        }
                        catch
                        {
                            //TODO: Remove this connection:
                        }
                    }
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
        }

        private IFramePayloadQueryReply MessageServer_OnQueryReceived(MessageServer server, Guid connectionId, IFramePayloadQuery payload)
        {
            try
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
                else if (payload is SiConfigure configure)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' configuration (v{configure.ClientVersion}).");

                    var session = Sessions.GetByConnectionId(connectionId);
                    if (session == null)
                    {
                        throw new Exception($"Session was not found for: '{connectionId}'.");
                    }

                    session.SetRemoteEndpointPort(configure.ClientListenUdpPort);

                    if (session.Endpoint == null)
                    {
                        throw new Exception($"Session endpoint can not be null: '{connectionId}'.");
                    }

                    _activeEndpoints.Add(connectionId, session.Endpoint);

                    return new SiConfigureReply()
                    {
                        ConnectionId = connectionId,
                        PlayerAbsoluteStateDelayMs = Settings.PlayerAbsoluteStateDelayMs
                    };
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else
                {
                    throw new NotImplementedException("The server query is not implemented.");
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex.Message);
                throw;

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

        private void MessageServer_OnConnected(MessageServer server, Guid connectionId, TcpClient tcpClient)
        {
            var remoteIPAddress = ((IPEndPoint?)tcpClient.Client.RemoteEndPoint)?.Address;
            if (remoteIPAddress == null)
            {
                server.Disconnect(connectionId);
                throw new Exception("The connection must expose an IP Address.");
            }

            var session = Sessions.Establish(connectionId, remoteIPAddress);
            Log.Verbose($"Accepted Connection: '{connectionId}'");
        }

        private void MessageServer_OnDisconnected(MessageServer server, Guid connectionId)
        {
            Sessions.Remove(connectionId);
            Log.Verbose($"Disconnected Connection: '{connectionId}'");
            _activeEndpoints.Remove(connectionId);
        }

        public void Stop()
        {
        }
    }
}
