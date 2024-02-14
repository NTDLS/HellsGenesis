using NTDLS.TightRPC;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Just a simple ping.
    /// </summary>
    public class SiPing : ITightRpcQuery<SiPingReply>
    {
        public DateTime Timestamp { get; set; }
        public Guid UID { get; set; }

        public SiPing()
        {
            Timestamp = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }
    }

    public class SiPingReply : ITightRpcQueryReply
    {
        public SiPing Ping { get; set; }

        public SiPingReply(SiPing ping)
        {
            Ping = ping;
        }
    }
}
