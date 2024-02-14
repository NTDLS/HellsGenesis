using NTDLS.TightRPC;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tell the server that the host has requested the lobby be deleted.
    /// </summary>
    public class SiDeleteLobby : ITightRpcNotification
    {
        public Guid LobbyUID { get; set; }

        public SiDeleteLobby(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
