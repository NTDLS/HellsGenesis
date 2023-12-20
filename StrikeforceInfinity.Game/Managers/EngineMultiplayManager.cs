using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Sprites.Enemies.Peons;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Shared.Messages.Query;
using StrikeforceInfinity.Shared.Payload;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// Contains everything you need to manage multiplayer functionality.
    /// </summary>
    internal class EngineMultiplayManager
    {
        public HgPlayMode PlayMode { get; private set; }
        public Guid LobbyUID { get; private set; }

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
                            _messageClient = messageClient;
                        }
                    }
                }
                #endregion
                return _messageClient;
            }
        }

        public EngineMultiplayManager(EngineCore gameCore)
        {
            _gameCore = gameCore;
        }

        public void GetSettingsFromServer()
        {
            //Tell the server hello and request any settings that the server wants to enforce on the client.
            var reply = MessageClient.Query<SiConfigureReply>(new SiConfigure()).Result;
            _gameCore.Settings.Multiplayer.PlayerAbsoluteStateDelayMs = reply.PlayerAbsoluteStateDelayMs;
        }

        public void SetPlayMode(HgPlayMode playMode) => PlayMode = playMode;
        public void RegisterLobbyUID(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(LobbyUID));
            }
        }

        public void SendLayoutFromLobbyOwner()
        {
            var sprites = new List<SiSpriteInfo>
                {
                    //Debugging code to add a few sprites to each connected client.
                    new SiSpriteInfo(typeof(SpriteEnemyPhoenix))
                    {
                        State = new SiSpriteAbsoluteState() { X = 100, Y = 100 }
                    },
                    new SiSpriteInfo(typeof(SpriteEnemyPhoenix))
                    {
                        State = new SiSpriteAbsoluteState() { X = 200, Y = 200 }
                    },
                    new SiSpriteInfo(typeof(SpriteEnemyPhoenix))
                    {
                        State = new SiSpriteAbsoluteState() { X = 300, Y = 300 }
                    }
                };

            MessageClient.Notify(new SiSituationLayout()
            {
                Sprites = sprites
            });
        }

        public void SetReadyToPlay()
        {
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiReadyToPlay());
            }
        }

        public void SetWaitingInLobby()
        {
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiWaitingInLobby());
            }
        }

        public async Task<SiLobbyInfo> GetLobbyInfo(Guid lobbyUID)
        {
            var reply = await MessageClient.Query<SiGetLobbyInfoReply>(new SiGetLobbyInfo() { LobyUID = lobbyUID });
            return reply.Info;
        }

        public Guid CreateHost(SiLobbyConfiguration gameHostConfiguration)
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

        private void MessageClient_OnNotificationReceived(MessageClient client, Guid connectionId, IFramePayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSituationLayout layoutDirective)
            {
                //The server is telling us to initialize the layout using the supplied sprites snd their states.

                foreach (var spriteInfo in layoutDirective.Sprites)
                {
                    var sprite = _gameCore.Sprites.Enemies.CreateByNameType(spriteInfo.FullTypeName);
                    sprite.MultiplayUID = spriteInfo.State.MultiplayUID;
                    sprite.X = spriteInfo.State.X;
                    sprite.Y = spriteInfo.State.Y;
                    sprite.Velocity.Angle.Degrees = spriteInfo.State.AngleDegrees;
                    sprite.Velocity.ThrottlePercentage = spriteInfo.State.ThrottlePercentage;
                    sprite.Velocity.BoostPercentage = spriteInfo.State.BoostPercentage;
                    sprite.ControlledBy = _gameCore.Multiplay.PlayMode switch
                    {
                        HgPlayMode.MutiPlayerHost => HgControlledBy.LocalAI,
                        HgPlayMode.MutiPlayerClient => HgControlledBy.Server,
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
            if (false)
            {

            }
            else
            {
                throw new NotImplementedException("The client query is not implemented.");
            }
        }

        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if (PlayMode != HgPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
            {
                MessageClient?.Notify(multiplayerEvent);
            }
        }
    }
}
