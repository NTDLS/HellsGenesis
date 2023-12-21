using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Query
{
    /// <summary>
    /// The client has connected to the server and is requesting any configuration.
    /// </summary>
    public class SiConfigure : IFramePayloadQuery
    {
        public int ClientListenUdpPort { get; set; }
    }

    public class SiConfigureReply : IFramePayloadQueryReply
    {
        /// <summary>
        /// The connection id of the tcp/ip connection according to the server.
        /// </summary>
        public Guid ConnectionId { get; set; }
        public int PlayerAbsoluteStateDelayMs { get; set; }
    }
}
