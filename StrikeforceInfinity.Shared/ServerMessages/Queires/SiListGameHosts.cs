using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Shared.ServerMessages.Queires
{
    /// <summary>
    /// Requests a list of game hosts from the server.
    /// </summary>
    public class SiListGameHosts : IFramePayloadQuery
    {
        //TODO: include some types of filters here
    }

    public class SiListGameHostsReply : IFramePayloadQueryReply
    {
        public List<SiGameHost> Collection { get; set; } = new();
    }
}
