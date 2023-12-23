using NTDLS.StreamFraming.Payloads;
using Si.Shared.Payload;

namespace Si.Shared.Messages.Query
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
