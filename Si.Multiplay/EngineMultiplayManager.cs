using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using Si.Shared;
using Si.Shared.Messages.Notify;
using Si.Shared.Messages.Query;
using Si.Shared.Payload;
using System.Reflection;
using static Si.Shared.SiConstants;

namespace Si.Multiplay
{
    /// <summary>
    /// The Multiplay manager is what we use to communicate with the Server. It is used by the game and the independent lobby hosts.
    /// </summary>
    public class EngineMultiplayManager
    {
        #region Events

        public delegate void ReceivedSituationLayout(SiSituationLayout situationLayout);
        /// <summary>
        /// Called when the multiplay manager receives a full layout of sprites and they need to be created on the map.
        /// </summary>
        public event ReceivedSituationLayout? OnReceivedSituationLayout;

        public delegate void SpriteVectorsUpdated(SiSpriteVectors spriteVectors);
        /// <summary>
        /// Called when the multiplay manager receives a updated list of sprite vectors for the connection.
        /// </summary>
        public event SpriteVectorsUpdated? OnSpriteVectorsUpdated;

        public delegate List<SiSpriteLayout> NeedSituationLayout();
        /// <summary>
        /// Called when the multiplay manager needs a layout of all applicable sprites create clone maps for each connection.
        /// </summary>
        public event NeedSituationLayout? OnNeedSituationLayout;

        public delegate void HostIsStartingGame();
        /// <summary>
        /// Called when the game is starting. Lets the clients know to close their lobby waiting menus.
        /// </summary>
        public event HostIsStartingGame? OnHostIsStartingGame;

        #endregion

        public MultiplayState State { get; private set; } = new();

        /// <summary>
        /// This is the UDP port that the client will listen on. This is communicated to the server via ConfigureConnection().
        /// </summary>
        private readonly int _clientListenUdpPort = UdpMessageManager.GetRandomUnusedUDPPort(5000, 8000);

        private readonly List<SiSpriteVector> _spriteVectorBuffer = new();
        private UdpMessageManager? _internal_udpManager;
        private MessageClient? _internal_messageClient;

        private UdpMessageManager UdpManager
        {
            get
            {
                #region UdpManager Creation and Connectivity.
                if (_internal_udpManager == null)
                {
                    lock (this)
                    {
                        _internal_udpManager ??= new UdpMessageManager(_clientListenUdpPort, UdpMessageManager_ProcessNotificationCallback);
                        _internal_udpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, new SiUDPHello());
                    }
                }
                return _internal_udpManager;
                #endregion
            }
        }

        private MessageClient MessageClient
        {
            get
            {
                #region Message Client Creation and Connectivity.
                if (_internal_messageClient == null)
                {
                    lock (this)
                    {
                        if (_internal_messageClient == null)
                        {
                            var messageClient = new MessageClient();

                            messageClient.OnNotificationReceived += MessageClient_OnNotificationReceived;
                            messageClient.OnQueryReceived += MessageClient_OnQueryReceived;

                            messageClient.Connect(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort);
                            _internal_messageClient = messageClient;

                            //Hit the property to force initilization of the upd manager.
                            SiUtility.EnsureNotNull(UdpManager);
                        }
                    }
                }
                #endregion
                return _internal_messageClient;
            }
        }

        public void NotifyHostIsStartingGame()
        {
            if (State.PlayMode == SiPlayMode.MutiPlayerHost)
            {
                MessageClient.Notify(new SiHostIsStartingGame(State.LobbyUID));
            }
        }

        /// <summary>
        /// Tells the server about the client and exchanges any applicable settings with/from the server.
        /// </summary>
        public void ConfigureConnection()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly?.GetName()?.Version;

            var query = new SiConfigure()
            {
                ClientLocalTime = DateTime.Now,
                ClientVersion = version,
                ClientListenUdpPort = _clientListenUdpPort
            };

            //Tell the server hello and request any settings that the server wants to enforce on the client.
            var reply = MessageClient.Query<SiConfigureReply>(query).Result;
            SiUtility.EnsureNotNull(reply);

            State.ConnectionId = reply.ConnectionId;
            State.PlayerAbsoluteStateDelayMs = reply.PlayerAbsoluteStateDelayMs;
        }

        /// <summary>
        /// This should be called from the lobby host client when a new level is
        /// loaded and the layout needs to be sent to all of the connected clients.
        /// This function checks to see if it is needed, so it is safe to call after loading any new situations, levels, etc.
        /// </summary>
        public void BroadcastSituationLayout()
        {
            if (State.PlayMode != SiPlayMode.MutiPlayerHost)
            {
                return;
            }

            var layouts = OnNeedSituationLayout?.Invoke();
            if (layouts == null)
            {
                layouts = new List<SiSpriteLayout>();
            }

            MessageClient.Notify(new SiSituationLayout()
            {
                Sprites = layouts
            });
        }

        /// <summary>
        /// Are we running as in single player mode, multiplay client or multiplay host?
        /// This should be called from the menus when starting a game.
        /// </summary>
        /// <param name="playMode"></param>
        public void SetPlayMode(SiPlayMode playMode)
            => State.PlayMode = playMode;

        /// <summary>
        /// Used by the host to delete a lobby from the server.
        /// </summary>
        /// <param name="lobbyUID"></param>
        public void DeleteLobby(Guid lobbyUID)
        {
            State.LobbyUID = lobbyUID;
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiDeleteLobby(State.LobbyUID));
            }
        }

        /// <summary>
        /// Called by both the client and the host to let the server know that it will be playing the game hosted by the given lobby.
        /// This lets the server know to wait on us to select a loadout and get ready.
        /// </summary>
        /// <param name="lobbyUID"></param>
        /// <param name="playerName"></param>
        public void RegisterLobbyUID(Guid lobbyUID, string playerName)
        {
            State.LobbyUID = lobbyUID;
            State.PlayerName = playerName;
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(State.LobbyUID, playerName));
            }
        }

        /// <summary>
        /// Lets the server know that this client is no longer going to be playing in this lobby.
        /// </summary>
        public void DeregisterLobbyUID()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiDeregisterToLobby(State.LobbyUID));
            }
            State.LobbyUID = Guid.Empty;
        }

        /// <summary>
        /// Lets the server know that the menues are closed, the player is on the screen and we are ready to receive the situation layout.
        /// </summary>
        public void SetReadyToPlay()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiReadyToPlay());
            }
        }

        public void SetLeftLobby()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiLeftLobby());
            }
        }

        /// <summary>
        /// /// Lets the server know that this client has finished selecting a loadout, or setting up the situation and is ready to start whenever.
        /// </summary>
        public void SetWaitingInLobby()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiWaitingInLobby());
            }
        }

        /// <summary>
        /// Gets basic information about a lobby. The name, number of connections
        /// and some details about each connection (such as player name and ping)
        /// </summary>
        /// <param name="lobbyUID"></param>
        /// <returns></returns>
        public async Task<SiLobbyInfo> GetLobbyInfo(Guid lobbyUID)
        {
            var reply = await MessageClient.Query<SiGetLobbyInfoReply>(new SiGetLobbyInfo() { LobbyUID = lobbyUID });
            SiUtility.EnsureNotNull(reply);
            return reply.Info;
        }

        /// <summary>
        /// Creates a new lobby that a player can join and play in with others.
        /// </summary>
        /// <param name="gameHostConfiguration"></param>
        /// <returns></returns>
        public Guid CreateLobby(SiLobbyConfiguration gameHostConfiguration)
        {
            var query = new SiCreateLobby()
            {
                Configuration = gameHostConfiguration
            };

            var reply = MessageClient.Query<SiCreateLobbyReply>(query).Result;
            SiUtility.EnsureNotNull(reply);
            return reply.LobbyUID;
        }

        /// <summary>
        /// Gets a list of lobbies from the server.
        /// </summary>
        /// <returns></returns>
        public List<SiLobbyConfiguration> ListLobbies()
        {
            var reply = MessageClient.Query<SiListLobbiesReply>(new SiListLobbies()).Result;
            SiUtility.EnsureNotNull(reply);
            return reply.Collection;
        }

        /// <summary>
        /// A UDP packet was received.
        /// </summary>
        /// <param name="payload"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UdpMessageManager_ProcessNotificationCallback(IUDPPayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSpriteVectors spriteVectors)
            {
                OnSpriteVectorsUpdated?.Invoke(spriteVectors);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else
            {
                throw new NotImplementedException("The client UPD notification is not implemented.");
            }
        }

        /// <summary>
        /// A one-way TCP/IP notification packet was received.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="connectionId"></param>
        /// <param name="payload"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MessageClient_OnNotificationReceived(MessageClient client, Guid connectionId, IFramePayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiHostIsStartingGame hostIsStartingGame)
            {
                OnHostIsStartingGame?.Invoke();
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiLobbyDeleted lobbyDeleted)
            {
                //TODO: The client is waiting in a lobby that no longer exists. We should do something.
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiSituationLayout situationLayout)
            {
                //The server is telling us to initialize the layout using the supplied sprites and their states.
                OnReceivedSituationLayout?.Invoke(situationLayout);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else
            {
                throw new NotImplementedException("The client TCP notification is not implemented.");
            }
        }

        /// <summary>
        /// A two-way TCP/IP wuery packet was received and expects a reply.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="connectionId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private IFramePayloadQueryReply MessageClient_OnQueryReceived(MessageClient client, Guid connectionId, IFramePayloadQuery payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiPing ping)
            {
                return new SiPingReply(ping);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else
            {
                throw new NotImplementedException("The client TCP query is not implemented.");
            }
        }

        /// <summary>
        /// Buffers sprite vector information so that all of the updates can be sent at one time at the end of the game loop.
        /// </summary>
        /// <param name="multiplayerEvent"></param>
        public void RecordSpriteVector(SiSpriteVector multiplayerEvent)
        {
            if (State.LobbyUID != Guid.Empty)
            {
                if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    _spriteVectorBuffer.Add(multiplayerEvent);
                }
            }
        }

        /// <summary>
        /// Sends the sprite vector buffer cache to the server so that it can be distributed to the clients.
        /// </summary>
        public void FlushSpriteVectorsToServer()
        {
            if (State.LobbyUID != Guid.Empty && _spriteVectorBuffer.Any())
            {
                if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    var spriteVectors = new SiSpriteVectors(_spriteVectorBuffer);

                    spriteVectors.ConnectionId = State.ConnectionId;

                    //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                    UdpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, spriteVectors);
                    _spriteVectorBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// Just a simple function used to send a notification with some checks.
        /// </summary>
        /// <param name="multiplayerEvent"></param>
        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
            {
                MessageClient?.Notify(multiplayerEvent);
            }
        }

        /// <summary>
        /// Used to cleanup any resources used by the multplay manager.
        /// </summary>
        public void Shutdown()
        {
            SiUtility.TryAndIgnore(() => UdpManager?.Shutdown());
            SiUtility.TryAndIgnore(() => _internal_messageClient?.Disconnect());
        }
    }
}
