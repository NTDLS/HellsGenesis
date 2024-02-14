using NTDLS.TightRPC;
using Si.Library.Messages.Notify;
using Si.Library.Messages.Query;

namespace Si.MultiplayClient
{
    internal class RpcClientNotificationHandlers : ITightRpcMessageHandler
    {
        private readonly EngineMultiplayManager _manager;

        public RpcClientNotificationHandlers(EngineMultiplayManager manager)
        {
            _manager = manager;
        }

        public void OnSiHostIsStartingGame(TightRpcContext context, SiHostIsStartingGame param)
        {
            _manager.InvokeHostIsStartingGame();
        }

        public void OnSiLobbyDeleted(TightRpcContext context, SiLobbyDeleted param)
        {
            //TODO: The client is waiting in a lobby that no longer exists. We should do something.
        }

        public void OnSiSituationLayout(TightRpcContext context, SiSituationLayout param)
        {
            //The server is telling us to initialize the layout using the supplied sprites and their states.
            _manager.InvokeReceivedLevelLayout(param);
        }

        public void OnSiPlayerSpriteCreated(TightRpcContext context, SiPlayerSpriteCreated param)
        {
            _manager.InvokePlayerSpriteCreated(param.SelectedPlayerClass, param.PlayerMultiplayUID);
        }

        public void OnSiSpriteCreated(TightRpcContext context, SiSpriteCreated param)
        {
            if (param.Layout != null)
            {
                _manager.InvokeSpriteCreated(param.Layout);
            }
        }

        public void OnSiHostStartedLevel(TightRpcContext context, SiHostStartedLevel param)
        {
            _manager.InvokeHostLevelStarted();
        }
    }
}