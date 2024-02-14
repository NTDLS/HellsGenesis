using NTDLS.TightRPC;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using Si.Library;
using Si.Library.Messages.Notify;
using Si.Library.Messages.Query;
using Si.Server.Core.Managers;
using Si.Server.Core.Objects;
using System.Net;

namespace Si.Server.Core
{
    internal class ServerEngineCore
    {
        /// <summary>
        /// A list of IpAddress endpoints for all connections, these are used for UDP sending.
        /// </summary>
        public Dictionary<Guid, IPEndPoint> ActiveEndpoints { get; private set; } = new();

        private readonly TightRpcServer _messageServer = new();
        private readonly UdpMessageManager _udpManager;

        public LogManager Log { get; private set; }
        public SessionManager Sessions { get; private set; }
        public LobbyManager Lobbies { get; private set; }
        public SiSettings Settings { get; private set; }

        public ServerEngineCore(SiSettings settings)
        {
            Settings = settings;

            _udpManager = new UdpMessageManager(Settings.DataPort, UdpMessageManager_ProcessNotificationCallback);

            Log = new LogManager(this);
            Sessions = new SessionManager(this);
            Lobbies = new LobbyManager(this);
        }

        public void Start()
        {
            _messageServer.AddHandler(new RpcServerQueryHandlers(this));
            _messageServer.AddHandler(new RpcServerNotificationHandlers(this));

            _messageServer.Start(Settings.DataPort);

            _messageServer.OnConnected += MessageServer_OnConnected;
            _messageServer.OnDisconnected += MessageServer_OnDisconnected;
            _messageServer.OnException += MessageServer_OnException;
        }

        private void MessageServer_OnException(TightRpcContext context, Exception ex, ITightRpcPayload? payload)
        {
            throw ex;
        }

        public async Task<SiPing?> PingConnection(Guid connectionId)
        {
            var result = await _messageServer.Query(connectionId, new SiPing(), 250);
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
                    if (ActiveEndpoints.TryGetValue(registeredConnectionId, out var ipEndpoint))
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

        /// <summary>
        /// Dispatches a message to all connections in the lobby exept for the one denoted by exceptConnectionId.
        /// </summary>
        public void BroadcastNotificationToAll(Lobby lobby, Guid exceptConnectionId, ITightRpcNotification message)
        {
            foreach (var connectionId in lobby.GetConnectionIDs().Where(o => o != exceptConnectionId))
            {
                _messageServer.Notify(connectionId, message);
            }
        }

        /// <summary>
        /// /// Dispatches a message to all connections in the lobby.
        /// </summary>
        /// <param name="lobby"></param>
        /// <param name="message"></param>
        private void BroadcastNotificationToAll(Lobby lobby, ITightRpcNotification message)
        {
            foreach (var connectionId in lobby.GetConnectionIDs())
            {
                _messageServer.Notify(connectionId, message);
            }
        }

        private void MessageServer_OnConnected(TightRpcContext context)
        {
            var remoteIPAddress = ((IPEndPoint?)context.TcpClient.Client.RemoteEndPoint)?.Address;
            if (remoteIPAddress == null)
            {
                context.Disconnect();
                throw new Exception("The connection must expose an IP Address.");
            }

            Sessions.Establish(context.ConnectionId, remoteIPAddress);
            Log.Verbose($"Accepted Connection: '{context.ConnectionId}'");
        }

        private void MessageServer_OnDisconnected(TightRpcContext context)
        {
            Sessions.Remove(context.ConnectionId);
            Log.Verbose($"Disconnected Connection: '{context.ConnectionId}'");
            ActiveEndpoints.Remove(context.ConnectionId);
        }

        public void Shutdown()
        {
            SiUtility.TryAndIgnore(() => _messageServer?.Stop());
            SiUtility.TryAndIgnore(() => _udpManager?.Shutdown());
        }
    }
}
