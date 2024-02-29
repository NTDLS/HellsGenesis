using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tells a connected client that a lobby that it is registred for has been deleted.
    /// </summary>
    public class SiLobbyDeleted : IRmNotification
    {
        public Guid LobbyUID { get; set; } = Guid.Empty;

        public SiLobbyDeleted(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
