using NTDLS.DatagramMessaging;
using Si.Library.Messages.Notify.Datagram;

namespace Si.Server.Core
{
    /// <summary>
    /// Recevies and processes time-sensitive messages from the multiplayer client (Si.MultiplayClient).
    /// </summary>
    internal class DatagramMessageNotificationHandlers : IDmMessageHandler
    {
        private readonly ServerEngineCore _serverCore;

        public DatagramMessageNotificationHandlers(ServerEngineCore serverCore)
        {
            _serverCore = serverCore;
        }

        public void OnSiHello(SiHello payload)
        {
            _serverCore.Log.Verbose($"A client sent a UDP hello.");
        }

        //------------------------------------------------------------------------------------------------------------------------------
        public void OnSiSpriteActions(DmContext context, SiSpriteActions spriteVectors)
        {
            if (!_serverCore.Sessions.TryGetByConnectionId(spriteVectors.ConnectionId, out var session))
            {
                _serverCore.Log.Exception($"The session was not found '{spriteVectors.ConnectionId}'.");
                return;
            }

            if (!_serverCore.Lobbies.TryGetByLobbyUID(session.LobbyUID, out var lobby))
            {
                _serverCore.Log.Exception($"The lobby was not found '{session.LobbyUID}'.");
                return;
            }

            //Broadcast the sprite vectors to all connections except for the one that sent it to us.
            foreach (var registeredConnectionId in lobby.GetConnectionIDs().Where(o => o != spriteVectors.ConnectionId))
            {
                if (_serverCore.ActiveEndpoints.TryGetValue(registeredConnectionId, out var ipEndpoint))
                {
                    try
                    {
                        context.Endpoint.WriteMessage(ipEndpoint, spriteVectors);
                    }
                    catch
                    {
                        //TODO: Remove this connection? Maybe track the number of errors and remove if too many occur?
                    }
                }
            }
        }
    }
}
