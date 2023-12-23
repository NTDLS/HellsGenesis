using NTDLS.UDPPacketFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// We need to send a UDP message to the server to populates the routers NAT table.
    /// </summary>
    public class SiUDPHello : IUDPPayloadNotification
    {
        public SiUDPHello()
        {
        }
    }
}
