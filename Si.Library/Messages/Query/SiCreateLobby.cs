using NTDLS.StreamFraming.Payloads;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
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
