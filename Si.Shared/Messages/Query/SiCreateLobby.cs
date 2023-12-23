using NTDLS.StreamFraming.Payloads;
using Si.Shared.Payload;

namespace Si.Shared.Messages.Query
{
    /// <summary>
    /// Tells the server to create a lobby.
    /// </summary>
    public class SiCreateLobby : IFramePayloadQuery
    {
        public SiLobbyConfiguration Configuration { get; set; } = new();
    }

    public class SiCreateLobbyReply : IFramePayloadQueryReply
    {
        public Guid LobbyUID { get; set; }
    }
}
