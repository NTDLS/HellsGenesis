using NTDLS.TightRPC;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// The host is yelling the server that it is starting the game. This can be in response to a manual start or an auto-start.
    /// </summary>
    public class SiHostIsStartingGame : ITightRpcNotification
    {
        public Guid LobbyUID { get; set; }

        public SiHostIsStartingGame(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
