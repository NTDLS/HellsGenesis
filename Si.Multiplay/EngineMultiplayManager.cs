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
    public class EngineMultiplayManager
    {
        #region Events

        public delegate void NeedSituationLayout(SiSituationLayout situationLayout);
        /// <summary>
        /// Called when the multiplay manager needs a layout of all applicable sprites states to fully update a client.
        /// </summary>
        public event NeedSituationLayout? OnReceivedSituationLayout;

        public delegate void SpriteVectorsUpdated(SiSpriteVectors spriteVectors);
        /// <summary>
        /// Called when the multiplay manager receives a updated list of sprite vectors for the client.
        /// </summary>
        public event SpriteVectorsUpdated? OnSpriteVectorsUpdated;

        public delegate List<SiSpriteLayout> NeedSpriteLayout();
        /// <summary>
        /// Called when the multiplay manager needs a layout of all applicable sprites states to fully update a client.
        /// </summary>
        public event NeedSpriteLayout? OnNeedSituationLayout;

        #endregion

        /// <summary>
        /// This is the UDP port that the client will listen on. This is communicated to the server via ConfigureConnection().
        /// </summary>
        private readonly int _clientListenUdpPort = UdpMessageManager.GetRandomUnusedUDPPort(5000, 8000);
        private readonly List<SiSpriteVector> _spriteVectorBuffer = new();
        private UdpMessageManager? _udpManager;

        private MessageClient? _messageClient;
        private MessageClient MessageClient
        {
            get
            {
                #region Message Client Creation and Connectivity.
                if (_messageClient == null)
                {
                    lock (this)
                    {
                        if (_messageClient == null)
                        {
                            var messageClient = new MessageClient();

                            messageClient.OnNotificationReceived += MessageClient_OnNotificationReceived;
                            messageClient.OnQueryReceived += MessageClient_OnQueryReceived;

                            messageClient.Connect(SiConstants.DataAddress, SiConstants.DataPort);

                            _udpManager = new UdpMessageManager(_clientListenUdpPort, UdpMessageManager_ProcessNotificationCallback);

                            _messageClient = messageClient;
                        }
                    }
                }
                #endregion
                return _messageClient;
            }
        }

        public MultiplayState State { get; private set; } = new();

        public EngineMultiplayManager()
        {
        }

        public void BroadcastLevelLayout()
        {
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
        /// Tells the server about the client and gets any applicable settings from the server.
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

        public void SetPlayMode(SiPlayMode playMode) => State.PlayMode = playMode;

        public void DeleteLobby(Guid lobbyUID)
        {
            State.LobbyUID = lobbyUID;
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiDeleteLobby(State.LobbyUID));
            }
        }

        public void RegisterLobbyUID(Guid lobbyUID, string playerName)
        {
            State.LobbyUID = lobbyUID;
            State.PlayerName = playerName;
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(State.LobbyUID, playerName));
            }
        }

        public void DeregisterLobbyUID()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiDeregisterToLobby(State.LobbyUID));
            }
            State.LobbyUID = Guid.Empty;
        }

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

        public void SetWaitingInLobby()
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiWaitingInLobby());
            }
        }

        public async Task<SiLobbyInfo> GetLobbyInfo(Guid lobbyUID)
        {
            var reply = await MessageClient.Query<SiGetLobbyInfoReply>(new SiGetLobbyInfo() { LobbyUID = lobbyUID });
            SiUtility.EnsureNotNull(reply);
            return reply.Info;
        }

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

        public List<SiLobbyConfiguration> GetHostList()
        {
            var reply = MessageClient.Query<SiListLobbiesReply>(new SiListLobbies()).Result;
            SiUtility.EnsureNotNull(reply);
            return reply.Collection;
        }

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

        private void MessageClient_OnNotificationReceived(MessageClient client, Guid connectionId, IFramePayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiLobbyDeleted lobbyDeleted)
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
                throw new NotImplementedException("The client notification is not implemented.");
            }
        }

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
                throw new NotImplementedException("The client query is not implemented.");
            }
        }
        public void NotifyddddSpriteVector(IFramePayloadNotification multiplayerEvent)
        {
            //TODO: We should really package these into a collection instead of sending one at a time.

            if (State.LobbyUID != Guid.Empty)
            {
                if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    MessageClient?.Notify(multiplayerEvent);
                }
            }
        }

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

        public void FlushSpriteVectorsToServer()
        {
            if (State.LobbyUID != Guid.Empty && _spriteVectorBuffer.Any())
            {
                if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true && _udpManager != null)
                {
                    var spriteVectors = new SiSpriteVectors(_spriteVectorBuffer);

                    spriteVectors.ConnectionId = State.ConnectionId;

                    //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                    _udpManager?.WriteMessage(SiConstants.DataAddress, SiConstants.DataPort, spriteVectors);
                    _spriteVectorBuffer.Clear();
                }
            }
        }

        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if (State.PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
            {
                MessageClient?.Notify(multiplayerEvent);
            }
        }

        public void Shutdown()
        {
            SiUtility.TryAndIgnore(() => _udpManager?.Shutdown());
            SiUtility.TryAndIgnore(() => _messageClient?.Disconnect());
        }

    }
}
