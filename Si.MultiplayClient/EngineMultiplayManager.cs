using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using Si.Shared;
using Si.Shared.Messages.Notify;
using Si.Shared.Messages.Query;
using Si.Shared.Payload;
using Si.Shared.Payload.DroneActions;
using System.Reflection;
using static Si.Shared.SiConstants;

namespace Si.MultiplayClient
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
        public event ReceivedSituationLayout? OnReceivedLevelLayout;

        public delegate void ApplySpriteActions(SiSpriteActions spriteActions);
        /// <summary>
        /// Called when the multiplay manager receives a updated list of sprite vectors for the connection.
        /// </summary>
        public event ApplySpriteActions? OnApplySpriteActions;

        public delegate List<SiSpriteLayout> NeedSituationLayout();
        /// <summary>
        /// Called when the multiplay manager needs a layout of all applicable sprites create clone maps for each connection.
        /// </summary>
        public event NeedSituationLayout? OnNeedLevelLayout;

        public delegate void HostIsStartingGame();
        /// <summary>
        /// Called when the game is starting. Lets the clients know to close their lobby waiting menus.
        /// </summary>
        public event HostIsStartingGame? OnHostIsStartingGame;

        public delegate void PlayerSpriteCreated(string selectedPlayerClass, Guid playerMultiplayUID);
        /// <summary>
        /// Called when the game is starting. Lets the clients know to close their lobby waiting menus.
        /// </summary>
        public event PlayerSpriteCreated? OnPlayerSpriteCreated;

        public delegate void HostLevelStarted();
        /// <summary>
        /// Called when the host owner starts the level and all connections should now show the player drones.
        /// </summary>
        public event HostLevelStarted? OnHostLevelStarted;

        public delegate void SpriteCreated(SiSpriteLayout layout);
        /// <summary>
        /// Called when any applicable sprite is created by a client and needs to be communictaed to other lobby clients.
        /// </summary>
        public event SpriteCreated? OnSpriteCreated;

        #endregion

        public MultiplayState State { get; private set; } = new();

        public bool ShouldRecordEvents
            => State.LobbyUID != Guid.Empty && State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true;

        /// <summary>
        /// This is the UDP port that the client will listen on. This is communicated to the server via ConfigureConnection().
        /// </summary>
        private readonly int _clientListenUdpPort = UdpMessageManager.GetRandomUnusedUDPPort(5000, 8000);

        private readonly List<SiDroneAction> _spriteActionBuffer = new();
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
                        _internal_udpManager.WriteMessage(SiConstants.MultiplayServerAddress, MultiplayServerTCPPort, new SiUDPHello());
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

        public void NotifySpriteCreated(SiSpriteLayout spriteLayout)
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                MessageClient.Notify(new SiSpriteCreated(spriteLayout));
            }
        }

        public void NotifyHostIsStartingGame()
        {
            if (State.PlayMode == SiPlayMode.MutiPlayerHost)
            {
                MessageClient.Notify(new SiHostIsStartingGame(State.LobbyUID));
            }
        }

        public void NotifyLevelStarted()
        {
            if (State.PlayMode == SiPlayMode.MutiPlayerHost)
            {
                MessageClient.Notify(new SiHostStartedLevel());
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

            var layouts = OnNeedLevelLayout?.Invoke();
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
                NotifyImmediately(new SiDeleteLobby(State.LobbyUID));
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
                NotifyImmediately(new SiRegisterToLobby(State.LobbyUID, playerName));
            }
        }

        /// <summary>
        /// Lets the server know that this client is no longer going to be playing in this lobby.
        /// </summary>
        public void DeregisterLobbyUID()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                NotifyImmediately(new SiDeregisterToLobby(State.LobbyUID));
            }
            State.LobbyUID = Guid.Empty;
        }

        /// <summary>
        /// Lets the server know that the menues are closed, the player is on the screen and we are ready to receive the situation layout.
        /// This is also used to tell all other connections what out player class and its multiplay UID is.
        /// </summary>
        public void NotifyPlayerSpriteCreated(Type selectedPlayerClass, Guid playerMultiplayUID)
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                NotifyImmediately(new SiPlayerSpriteCreated(selectedPlayerClass.Name, playerMultiplayUID));
            }
        }

        public void SetLeftLobby()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                NotifyImmediately(new SiLeftLobby());
            }
        }

        /// <summary>
        /// /// Lets the server know that this client has finished selecting a loadout, or setting up the situation and is ready to start whenever.
        /// </summary>
        public void SetWaitingInLobby()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                NotifyImmediately(new SiWaitingInLobby());
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
            if (payload is SiSpriteActions spriteActions)
            {
                OnApplySpriteActions?.Invoke(spriteActions);
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
                OnReceivedLevelLayout?.Invoke(situationLayout);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiPlayerSpriteCreated playerSpriteCreated)
            {
                OnPlayerSpriteCreated?.Invoke(playerSpriteCreated.SelectedPlayerClass, playerSpriteCreated.PlayerMultiplayUID);
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiSpriteCreated spriteCreated)
            {
                if (spriteCreated.Layout != null)
                {
                    OnSpriteCreated?.Invoke(spriteCreated.Layout);
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiHostStartedLevel)
            {
                OnHostLevelStarted?.Invoke();
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
        /// <param name="multiplayEvent"></param>
        public void RecordSpriteVector(SiDroneActionVector multiplayEvent)
        {
            if (ShouldRecordEvents)
            {
                _spriteActionBuffer.Add(multiplayEvent);
            }
        }

        /// <summary>
        /// Buffers sprite vector information so that all of the updates can be sent at one time at the end of the game loop.
        /// </summary>
        /// <param name="multiplayerEvent"></param>
        public void RecordSpriteWeaponFire(SiDroneActionFireWeapon multiplayEvent)
        {
            if (ShouldRecordEvents)
            {
                _spriteActionBuffer.Add(multiplayEvent);
            }
        }

        /// <summary>
        /// Buffers sprite vector information so that all of the updates can be sent at one time at the end of the game loop.
        /// </summary>
        /// <param name="multiplayerEvent"></param>
        public void RecordSpriteExplode(Guid playerMultiplayUID)
        {
            if (ShouldRecordEvents)
            {
                _spriteActionBuffer.Add(new SiDroneActionExplode(playerMultiplayUID));
            }
        }

        public void RecordSpriteHit(Guid playerMultiplayUID)
        {
            if (ShouldRecordEvents)
            {
                _spriteActionBuffer.Add(new SiDroneActionHit(playerMultiplayUID));
            }
        }

        /// <summary>
        /// Sends the sprite vector buffer cache to the server so that it can be distributed to the clients.
        /// </summary>
        public void FlushSpriteVectorsToServer()
        {
            if (State.LobbyUID != Guid.Empty && _spriteActionBuffer.Any())
            {
                if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    var spriteActions = new SiSpriteActions(_spriteActionBuffer);

                    spriteActions.ConnectionId = State.ConnectionId;

                    //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                    UdpManager.WriteMessage(SiConstants.MultiplayServerAddress, SiConstants.MultiplayServerTCPPort, spriteActions);
                    _spriteActionBuffer.Clear();
                }
            }
        }

        /// <summary>
        /// Just a simple function used to send a notification with some checks.
        /// </summary>
        /// <param name="multiplayerEvent"></param>
        public void NotifyImmediately(IFramePayloadNotification multiplayerEvent)
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
