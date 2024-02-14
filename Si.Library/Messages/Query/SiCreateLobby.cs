using NTDLS.TightRPC;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Tells the server to create a lobby.
    /// </summary>
    public class SiCreateLobby : ITightRpcQuery<SiCreateLobbyReply>
    {
        public SiLobbyConfiguration Configuration { get; set; } = new();
    }

    public class SiCreateLobbyReply : ITightRpcQueryReply
    {
        public Guid LobbyUID { get; set; }
    }
}
