using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tell the server that a connection is leaving the lobby entirely.
    /// </summary>
    public class SiDeregisterToLobby : IRmNotification
    {
        public Guid LobbyUID { get; set; }

        public SiDeregisterToLobby(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
