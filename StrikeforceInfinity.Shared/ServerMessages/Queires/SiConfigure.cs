using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.ServerMessages.Queires
{
    /// <summary>
    /// The client has connected to the server and is requesting any configuration.
    /// </summary>
    public class SiConfigure : IFramePayloadQuery
    {
        //TODO: include some types of filters here
    }

    public class SiConfigureReply : IFramePayloadQueryReply
    {
        public int PlayerAbsoluteStateDelayMs { get; set; }
    }
}
