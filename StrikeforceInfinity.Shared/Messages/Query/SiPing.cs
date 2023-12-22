using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Query
{
    /// <summary>
    /// Just a simple ping.
    /// </summary>
    public class SiPing : IFramePayloadQuery
    {
        public DateTime Timestamp { get; set; }
        public Guid UID { get; set; }

        public SiPing()
        {
            Timestamp = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }
    }

    public class SiPingReply : IFramePayloadQueryReply
    {
        public SiPing Ping { get; set; }

        public SiPingReply(SiPing ping)
        {
            Ping = ping;
        }
    }
}
