using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using StrikeforceInfinity.Engine;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Shared;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Shared.Messages.Query;
using StrikeforceInfinity.Shared.Payload;
using StrikeforceInfinity.Sprites.BasesAndInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// Contains everything you need to manage multiplayer functionality.
    /// </summary>
    internal class EngineMultiplayManager
    {
        /// <summary>
        /// This is the UDP port that the client will listen on. This is communicated to the server via ConfigureConnection().
        /// </summary>
        private readonly int _clientListenUdpPort = UdpMessageManager.GetRandomUnusedUDPPort(5000, 8000);
        private readonly List<SiSpriteVector> _spriteVectorBuffer = new();
        private UdpMessageManager _udpManager;
        private readonly EngineCore _gameCore;
        private MessageClient _messageClient;
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

                            messageClient.Connect(Constants.DataAddress, Constants.DataPort);

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
        public SiPlayMode PlayMode { get; private set; }
        public Guid LobbyUID { get; private set; } = Guid.Empty;

        public EngineMultiplayManager(EngineCore gameCore)
        {
            _gameCore = gameCore;
        }

        /// <summary>
        /// Tells the server about the client and gets any applicable settings from the server.
        /// </summary>
        public void ConfigureConnection()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Version version = assembly.GetName().Version;

            var query = new SiConfigure()
            {
                ClientLocalTime = DateTime.Now,
                ClientVersion = version,
                ClientListenUdpPort = _clientListenUdpPort
            };

            //Tell the server hello and request any settings that the server wants to enforce on the client.
            var reply = MessageClient.Query<SiConfigureReply>(query).Result;

            _gameCore.Multiplay.State.ConnectionId = reply.ConnectionId;
            _gameCore.Multiplay.State.PlayerAbsoluteStateDelayMs = reply.PlayerAbsoluteStateDelayMs;
        }

        public void SetPlayMode(SiPlayMode playMode) => PlayMode = playMode;

        public void DeleteLobby(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiDeleteLobby(LobbyUID));
            }
        }

        public void RegisterLobbyUID(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(LobbyUID));
            }
        }

        public void DeregisterLobbyUID()
        {
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(LobbyUID));
            }
            LobbyUID = Guid.Empty;
        }

        public void BroadcastLevelLayout()
        {
            var spriteLayouts = new List<SiSpriteLayout>();

            //--------------------------------------------------------------------------------------
            //-- Send the enemy sprites:
            //--------------------------------------------------------------------------------------
            var enemies = _gameCore.Sprites.Enemies.All();
            foreach (var enemy in enemies)
            {
                //Make sure the MultiplayUID matches the MultiplayUID at all other connections.
                enemy.MultiplayUID = enemy.UID;

                spriteLayouts.Add(new SiSpriteLayout(enemy.GetType().FullName + "Drone", enemy.UID)
                {
                    Vector = new SiSpriteVector() { X = enemy.X, Y = enemy.Y }
                });
            }

            //--------------------------------------------------------------------------------------
            //-- Send the human player sprite (drone):
            //--------------------------------------------------------------------------------------
            //Make sure the MultiplayUID matches the MultiplayUID at all other connections.
            _gameCore.Player.Sprite.MultiplayUID = _gameCore.Player.Sprite.UID;
            spriteLayouts.Add(new SiSpriteLayout(_gameCore.Player.Sprite.GetType().FullName + "Drone", _gameCore.Player.Sprite.UID)
            {
                Vector = new SiSpriteVector() { X = _gameCore.Display.BackgroundOffset.X, Y = _gameCore.Display.BackgroundOffset.Y }
            });

            //--------------------------------------------------------------------------------------
            MessageClient.Notify(new SiSituationLayout()
            {
                Sprites = spriteLayouts
            });
        }

        public void SetReadyToPlay()
        {
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiReadyToPlay());
            }
        }

        public void SetLeftLobby()
        {
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiLeftLobby());
            }
        }

        public void SetWaitingInLobby()
        {
            if (PlayMode != SiPlayMode.SinglePlayer)
            {
                Notify(new SiWaitingInLobby());
            }
        }

        public async Task<SiLobbyInfo> GetLobbyInfo(Guid lobbyUID)
        {
            var reply = await MessageClient.Query<SiGetLobbyInfoReply>(new SiGetLobbyInfo() { LobbyUID = lobbyUID });
            return reply.Info;
        }

        public Guid CreateLobby(SiLobbyConfiguration gameHostConfiguration)
        {
            var query = new SiCreateLobby()
            {
                Configuration = gameHostConfiguration
            };

            var reply = MessageClient.Query<SiCreateLobbyReply>(query).Result;

            return reply.LobbyUID;
        }

        public List<SiLobbyConfiguration> GetHostList()
        {
            var reply = MessageClient.Query<SiListLobbiesReply>(new SiListLobbies()).Result;
            return reply.Collection;
        }

        private void UdpMessageManager_ProcessNotificationCallback(IUDPPayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSpriteVectors spriteVectors)
            {
                var allMultiplayUIDs = spriteVectors.Collection.Select(o => o.MultiplayUID).ToHashSet();

                //Get all the sprites ahead of time. I "think" this is faster than searching in a loop.
                var sprites = _gameCore.Sprites.Collection.Where(o => allMultiplayUIDs.Contains(o.MultiplayUID)).ToList();

                foreach (var vector in spriteVectors.Collection)
                {
                    var sprite = sprites.Where(o => o.MultiplayUID == vector.MultiplayUID).FirstOrDefault();
                    if (sprite != null)
                    {
                        if (sprite is ISpriteDrone playerDrone)
                        {
                            playerDrone.ApplyMultiplayVector(vector);
                        }
                        else
                        {
                        }
                    }
                }
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
            else if (payload is SiSituationLayout layoutDirective)
            {
                //The server is telling us to initialize the layout using the supplied sprites and their states.

                _gameCore.Sprites.Enemies.DeleteAll();

                foreach (var spriteInfo in layoutDirective.Sprites)
                {
                    Debug.WriteLine($"Adding Sprite: {spriteInfo.MultiplayUID}->'{spriteInfo.FullTypeName}'");

                    var sprite = _gameCore.Sprites.CreateByNameOfType(spriteInfo.FullTypeName);
                    sprite.MultiplayUID = spriteInfo.MultiplayUID;
                    sprite.X = spriteInfo.Vector.X;
                    sprite.Y = spriteInfo.Vector.Y;
                    sprite.Velocity.Angle.Degrees = spriteInfo.Vector.AngleDegrees;
                    sprite.Velocity.ThrottlePercentage = spriteInfo.Vector.ThrottlePercentage;
                    sprite.Velocity.BoostPercentage = spriteInfo.Vector.BoostPercentage;
                    sprite.ControlledBy = _gameCore.Multiplay.PlayMode switch
                    {
                        SiPlayMode.MutiPlayerHost => SiControlledBy.LocalAI,
                        SiPlayMode.MutiPlayerClient => SiControlledBy.Server,
                        _ => throw new InvalidOperationException("Unhandled PlayMode")
                    };
                }
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

            if (LobbyUID != Guid.Empty)
            {
                if (PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    MessageClient?.Notify(multiplayerEvent);
                }
            }
        }


        public void RecordSpriteVector(SiSpriteVector multiplayerEvent)
        {
            if (LobbyUID != Guid.Empty)
            {
                if (PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    _spriteVectorBuffer.Add(multiplayerEvent);
                }
            }
        }

        public void FlushSpriteVectorsToServer()
        {
            if (LobbyUID != Guid.Empty && _spriteVectorBuffer.Any())
            {
                if (PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true && _udpManager != null)
                {
                    var spriteVectors = new SiSpriteVectors(_spriteVectorBuffer);

                    spriteVectors.ConnectionId = State.ConnectionId;

                    //System.Diagnostics.Debug.WriteLine($"MultiplayUID: {_spriteVectors.Select(o=>o.MultiplayUID).Distinct().Count()}");
                    _udpManager?.WriteMessage(Constants.DataAddress, Constants.DataPort, spriteVectors);
                    _spriteVectorBuffer.Clear();
                }
            }
        }

        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if (PlayMode != SiPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
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
