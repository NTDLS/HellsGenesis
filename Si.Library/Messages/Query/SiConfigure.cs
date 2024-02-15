using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// The client has connected and the server and client are exchanging configuration/settings.
    /// </summary>
    public class SiConfigure : IRmQuery<SiConfigureReply>
    {
        /// <summary>
        /// The port which the client will be listening to for UDP communications from the server.
        /// </summary>
        public int ClientListenUdpPort { get; set; }

        /// <summary>
        /// The version of the client program.
        /// </summary>
        public Version? ClientVersion { get; set; }

        /// <summary>
        /// The local time for the client.
        /// </summary>
        public DateTime? ClientLocalTime { get; set; }
    }

    public class SiConfigureReply : IRmQueryReply
    {
        /// <summary>
        /// The connection id of the tcp/ip connection according to the server.
        /// </summary>
        public Guid ConnectionId { get; set; }
        public int PlayerAbsoluteStateDelayMs { get; set; }
    }
}
