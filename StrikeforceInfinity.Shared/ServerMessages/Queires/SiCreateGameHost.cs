using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Shared.ServerMessages.Queires
{
    /// <summary>
    /// Tells the server to create a game host.
    /// </summary>
    public class SiCreateGameHost : IFramePayloadQuery
    {
        public SiGameHost Configuration { get; set; } = new();
    }

    public class SiCreateGameHostReply : IFramePayloadQueryReply
    {
        public Guid UID { get; set; }
    }
}
