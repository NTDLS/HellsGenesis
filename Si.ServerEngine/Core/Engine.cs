using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using Si.ServerEngine.Core.Managers;
using Si.ServerEngine.Core.Objects;
using Si.Shared;
using Si.Shared.Messages.Notify;
using Si.Shared.Messages.Query;
using Si.Shared.Payload;
using System.Net;
using System.Net.Sockets;

namespace Si.ServerEngine.Core
{
    internal class Engine
    {
        /// <summary>
        /// A list of IPaddress endpoints for all connections, these are used for UDP sending.
        /// </summary>
        private readonly Dictionary<Guid, IPEndPoint> _activeEndpoints = new();
        private readonly MessageServer _messageServer = new();
        private readonly UdpMessageManager _udpManager;

        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public LobbyManager Lobbies { get; private set; }
        public SiSettings Settings { get; private set; }

        public Engine(SiSettings settings)
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
            _messageServer.OnException += MessageServer_OnException;
        }

        private void MessageServer_OnException(MessageServer client, Guid connectionId, Exception ex, IFramePayload? payload)
        {
            throw ex;
        }

        public async Task<SiPing?> PingConnection(Guid connectionId)
        {
            var result = await _messageServer.Query<SiPingReply>(connectionId, new SiPing(), 250);
            return result?.Ping;
        }

        private void UdpMessageManager_ProcessNotificationCallback(IUDPPayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiUDPHello)
            {
                Log.Verbose($"A client sent a UDP hello.");
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiSpriteActions spriteVectors)
            {
                if (!Sessions.TryGetByConnectionId(spriteVectors.ConnectionId, out var session))
                {
                    Log.Exception($"The session was not found '{spriteVectors.ConnectionId}'.");
                    return;
                }

                if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                {
                    Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                    return;
                }

                //Broadcast the sprite vectors all connections except for the one that sent it to us.
                foreach (var registeredConnectionId in lobby.GetConnectionIDs().Where(o => o != spriteVectors.ConnectionId))
                {
                    if (_activeEndpoints.TryGetValue(registeredConnectionId, out var ipEndpoint))
                    {
                        try
                        {
                            _udpManager.WriteMessage(ipEndpoint, spriteVectors);
                        }
                        catch
                        {
                            //TODO: Remove this connection? Maybe track the number of errors and remove if too many occur?
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
                    Log.Verbose($"ConnectionId: '{connectionId}' getting lobby info for '{getLobbyInfo.LobbyUID}'.");

                    if (!Lobbies.TryGetByLobbyUID(getLobbyInfo.LobbyUID, out var lobby))
                    {
                        throw new Exception($"The lobby was not found '{getLobbyInfo.LobbyUID}'.");
                    }

                    return new SiGetLobbyInfoReply()
                    {
                        Info = new SiLobbyInfo
                        {
                            Name = lobby.Name,
                            IsHeadless = lobby.IsHeadless,
                            RemainingSecondsUntilAutoStart = lobby.RemainingSecondsUntilAutoStart,
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

                    var lobbies = Lobbies.GetList();

                    return new SiListLobbiesReply()
                    {
                        Collection = lobbies
                    };
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiConfigure configure)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' configuration (v{configure.ClientVersion}).");

                    if (!Sessions.TryGetByConnectionId(connectionId, out var session))
                    {
                        throw new Exception($"Session was not found for: '{connectionId}'.");
                    }

                    session.SetRemoteEndpointPort(configure.ClientListenUdpPort);
                    if (session.Endpoint == null)
                    {
                        throw new Exception($"Session endpoint can not be null: '{connectionId}'.");
                    }

                    _activeEndpoints.Remove(connectionId);
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
            try
            {
                if (!Sessions.TryGetByConnectionId(connectionId, out var session))
                {
                    Log.Exception($"The session was not found '{connectionId}'.");
                    return;
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiHostIsStartingGame hostIsStartingGame)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' is starting game for lobby: '{hostIsStartingGame.LobbyUID}'.");

                    if (!Lobbies.TryGetByLobbyUID(hostIsStartingGame.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{hostIsStartingGame.LobbyUID}'.");
                        return;
                    }

                    //Let all the non-lobby-owner connections know that the host is starting the game.
                    BroadcastNotificationToAllBut(lobby, connectionId, hostIsStartingGame);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiDeleteLobby deleteLobby)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' is deleting the lobby: '{deleteLobby.LobbyUID}'");

                    if (!Lobbies.TryGetByLobbyUID(deleteLobby.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{deleteLobby.LobbyUID}'.");
                        return;
                    }

                    BroadcastNotificationToAllBut(lobby, connectionId, new SiLobbyDeleted(deleteLobby.LobbyUID));

                    Lobbies.Delete(deleteLobby.LobbyUID);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiDeregisterToLobby deregister)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' deregistered for lobby: '{deregister.LobbyUID}'");

                    if (!Lobbies.TryGetByLobbyUID(deregister.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{deregister.LobbyUID}'.");
                        return;
                    }

                    lobby.Deregister(connectionId);
                    session.ClearCurrentLobby();
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiRegisterToLobby register)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' registered for lobby: '{register.LobbyUID}'");

                    if (!Lobbies.TryGetByLobbyUID(register.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{register.LobbyUID}'.");
                        return;
                    }

                    lobby.Register(connectionId, register.PlayerName);
                    session.SetCurrentLobby(register.LobbyUID);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiWaitingInLobby waitingInLobby)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' is waiting in the lobby.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    lobby.SetConnectionAsWaitingInLobby(connectionId);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiLeftLobby)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' has left the lobby.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    lobby.SetConnectionAsLeftInLobby(connectionId);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiSituationLayout layoutDirective)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' issued a new layout.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    //The owner of the lobby sent a new layout, send it to all of the connections except for the one that sent it to us.
                    BroadcastNotificationToAllBut(lobby, connectionId, layoutDirective);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiPlayerSpriteCreated playerSpriteCreated)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' has created its sprite.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    BroadcastNotificationToAllBut(lobby, connectionId, playerSpriteCreated);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiHostStartedLevel hostStartedLevel)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' is started level.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    BroadcastNotificationToAllBut(lobby, connectionId, hostStartedLevel);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else if (payload is SiSpriteCreated spriteCreated)
                {
                    Log.Verbose($"ConnectionId: '{connectionId}' created a sprite of type {spriteCreated.Layout?.FullTypeName ?? string.Empty}.");

                    if (!Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
                    {
                        Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                        return;
                    }

                    BroadcastNotificationToAllBut(lobby, connectionId, spriteCreated);
                }
                //------------------------------------------------------------------------------------------------------------------------------
                else
                {
                    throw new NotImplementedException("The server notification is not implemented.");
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex.Message);
                throw;
            }
        }

        private void BroadcastNotificationToAllBut(Lobby lobby, Guid exceptConnectionId, IFramePayloadNotification message)
        {
            foreach (var connectionId in lobby.GetConnectionIDs().Where(o => o != exceptConnectionId))
            {
                _messageServer.Notify(connectionId, message);
            }
        }

        private void BroadcastNotificationToAll(Lobby lobby, IFramePayloadNotification message)
        {
            foreach (var connectionId in lobby.GetConnectionIDs())
            {
                _messageServer.Notify(connectionId, message);
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

            Sessions.Establish(connectionId, remoteIPAddress);
            Log.Verbose($"Accepted Connection: '{connectionId}'");
        }

        private void MessageServer_OnDisconnected(MessageServer server, Guid connectionId)
        {
            Sessions.Remove(connectionId);
            Log.Verbose($"Disconnected Connection: '{connectionId}'");
            _activeEndpoints.Remove(connectionId);
        }

        public void Shutdown()
        {
            SiUtility.TryAndIgnore(() => _messageServer?.Stop());
            SiUtility.TryAndIgnore(() => _udpManager?.Shutdown());
        }
    }
}
