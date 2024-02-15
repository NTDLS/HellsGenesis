using NTDLS.ReliableMessaging;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Tells the server to create a lobby.
    /// </summary>
    public class SiCreateLobby : IRmQuery<SiCreateLobbyReply>
    {
        public SiLobbyConfiguration Configuration { get; set; } = new();
    }

    public class SiCreateLobbyReply : IRmQueryReply
    {
        public Guid LobbyUID { get; set; }
    }
}
