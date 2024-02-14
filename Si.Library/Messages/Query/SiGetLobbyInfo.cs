using NTDLS.TightRPC;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Gets some basic information about a lobby.
    /// </summary>
    public class SiGetLobbyInfo : ITightRpcQuery<SiGetLobbyInfoReply>
    {
        public Guid LobbyUID { get; set; }
    }

    public class SiGetLobbyInfoReply : ITightRpcQueryReply
    {
        public SiLobbyInfo Info { get; set; } = new();
    }
}
