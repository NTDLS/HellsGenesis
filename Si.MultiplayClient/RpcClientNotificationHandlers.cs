using NTDLS.ReliableMessaging;
using Si.Library.Messages.Notify;

namespace Si.MultiplayClient
{
    internal class RpcClientNotificationHandlers : IRmMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public RpcClientNotificationHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public void OnSiHostIsStartingGame(RmContext context, SiHostIsStartingGame param)
        {
            _manager.InvokeHostIsStartingGame();
        }

        public void OnSiLobbyDeleted(RmContext context, SiLobbyDeleted param)
        {
            //TODO: The client is waiting in a lobby that no longer exists. We should do something.
        }

        public void OnSiSituationLayout(RmContext context, SiSituationLayout param)
        {
            //The server is telling us to initialize the layout using the supplied sprites and their states.
            _manager.InvokeReceivedLevelLayout(param);
        }

        public void OnSiPlayerSpriteCreated(RmContext context, SiPlayerSpriteCreated param)
        {
            _manager.InvokePlayerSpriteCreated(param.SelectedPlayerClass, param.PlayerMultiplayUID);
        }

        public void OnSiSpriteCreated(RmContext context, SiSpriteCreated param)
        {
            if (param.Layout != null)
            {
                _manager.InvokeSpriteCreated(param.Layout);
            }
        }

        public void OnSiHostStartedLevel(RmContext context, SiHostStartedLevel param)
        {
            _manager.InvokeHostLevelStarted();
        }
    }
}