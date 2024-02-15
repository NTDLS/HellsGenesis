using NTDLS.DatagramMessaging;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// We need to send a UDP message to the server to populates the routers NAT table.
    /// </summary>
    public class SiUDPHello : IDmNotification
    {
        public SiUDPHello()
        {
        }
    }
}
