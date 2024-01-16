using NTDLS.StreamFraming.Payloads;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Gets some basic information about a lobby.
    /// </summary>
    public class SiGetLobbyInfo : IFramePayloadQuery
    {
        public Guid LobbyUID { get; set; }
    }

    public class SiGetLobbyInfoReply : IFramePayloadQueryReply
    {
        public SiLobbyInfo Info { get; set; } = new();
    }
}
