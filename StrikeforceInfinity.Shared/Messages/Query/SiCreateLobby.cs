using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Shared.Messages.Query
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
        public Guid UID { get; set; }
    }
}
