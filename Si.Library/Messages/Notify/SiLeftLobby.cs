using NTDLS.TightRPC;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection has left the lobby.
    /// They are still registered to play in the lobby, but they are selecting a new loadout.
    /// </summary>
    public class SiLeftLobby : ITightRpcNotification
    {
    }
}
