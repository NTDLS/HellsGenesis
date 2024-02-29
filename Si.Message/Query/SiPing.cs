using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Just a simple ping.
    /// </summary>
    public class SiPing : IRmQuery<SiPingReply>
    {
        public DateTime Timestamp { get; set; }
        public Guid UID { get; set; }

        public SiPing()
        {
            Timestamp = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }
    }

    public class SiPingReply : IRmQueryReply
    {
        public SiPing Ping { get; set; }

        public SiPingReply(SiPing ping)
        {
            Ping = ping;
        }
    }
}
