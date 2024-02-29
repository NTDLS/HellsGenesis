using NTDLS.ReliableMessaging;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Gets some basic information about a lobby.
    /// </summary>
    public class SiGetLobbyInfo : IRmQuery<SiGetLobbyInfoReply>
    {
        public Guid LobbyUID { get; set; }
    }

    public class SiGetLobbyInfoReply : IRmQueryReply
    {
        public SiLobbyInfo Info { get; set; } = new();
    }
}
