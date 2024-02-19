using NTDLS.ReliableMessaging;
using Si.Library.Messages.Notify;

namespace Si.MultiplayClient
{
    /// <summary>
    /// Recevies and processes time-sensitive messages from the multiplayer host.
    /// </summary>
    internal class ReliableMessageNotificationHandlers : IRmMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public ReliableMessageNotificationHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public void OnSiHostIsStartingGame(SiHostIsStartingGame param)
        {
            _manager.InvokeHostIsStartingGame();
        }

        public void OnSiLobbyDeleted(SiLobbyDeleted param)
        {
            //TODO: The client is waiting in a lobby that no longer exists. We should do something.
        }

        public void OnSiSituationLayout(SiSituationLayout param)
        {
            //The server is telling us to initialize the layout using the supplied sprites and their states.
            _manager.InvokeReceivedLevelLayout(param);
        }

        public void OnSiPlayerSpriteCreated(SiPlayerSpriteCreated param)
        {
            _manager.InvokePlayerSpriteCreated(param.SelectedPlayerClass, param.PlayerMultiplayUID);
        }

        public void OnSiSpriteCreated(SiSpriteCreated param)
        {
            if (param.Layout != null)
            {
                _manager.InvokeSpriteCreated(param.Layout);
            }
        }

        public void OnSiHostStartedLevel(SiHostStartedLevel param)
        {
            _manager.InvokeHostLevelStarted();
        }
    }
}
