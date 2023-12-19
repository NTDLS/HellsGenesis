using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Shared.Payload;
using StrikeforceInfinity.Shared.ServerMessages.Queires;
using System;
using System.Collections.Generic;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// 
    /// </summary>
    internal class EngineMultiplayManager
    {
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

        public void Configure()
        {
            //Tell the server hello and request any settings that the server wants to enforce on the client.
            var reply = MessageClient.Query<SiConfigureReply>(new SiConfigure()).Result;
            _gameCore.Settings.Multiplayer.PlayerAbsoluteStateDelayMs = reply.PlayerAbsoluteStateDelayMs;
        }

        public void CreateHost(SiGameHost gameHostConfiguration)
        {
            var query = new SiCreateGameHost()
            {
                Configuration = gameHostConfiguration
            };

            var reply = MessageClient.Query<SiCreateGameHostReply>(query).Result;

            _gameCore.SetGameHostUID(reply.UID);
        }

        public List<SiGameHost> GetHostList()
        {
            var reply = MessageClient.Query<SiListGameHostsReply>(new SiListGameHosts()).Result;
            return reply.Collection;
        }

        private void MessageClient_OnNotificationReceived(MessageClient client, Guid connectionId, IFramePayloadNotification payload)
        {

        }

        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if ((_gameCore.PlayMode == HgPlayMode.MutiPlayerHost || _gameCore.PlayMode == HgPlayMode.MutiPlayerClient) && MessageClient?.IsConnected == true)
            {
                MessageClient?.Notify(multiplayerEvent);
            }
        }


    }
}
