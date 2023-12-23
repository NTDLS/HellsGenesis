using NTDLS.StreamFraming.Payloads;
using Si.Shared.Payload;

namespace Si.Shared.Messages.Query
{
    /// <summary>
    /// Requests a list of lobbies from the server.
    /// </summary>
    public class SiListLobbies : IFramePayloadQuery
    {
        //TODO: include some types of filters here
    }

    public class SiListLobbiesReply : IFramePayloadQueryReply
    {
        public List<SiLobbyConfiguration> Collection { get; set; } = new();
    }
}
