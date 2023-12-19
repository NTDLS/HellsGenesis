using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Shared.Messages.Query
{
    /// <summary>
    /// Let the client that owns the lobby know what we are ready for the configuration.
    /// The configuration consists of all initial non-player sprites.
    /// </summary>
    public class SiRequestLayoutFromLobbyOwner : IFramePayloadQuery
    {
    }

    public class SiRequestLayoutFromLobbyOwnerReply : IFramePayloadQueryReply
    {
        public List<SiSpriteInfo> Sprites { get; set; } = new();
    }
}
