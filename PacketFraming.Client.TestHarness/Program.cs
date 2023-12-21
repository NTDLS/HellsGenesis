using NTDLS.UDPPacketFraming;
using PacketFraming.Test.Shared;
using System.Net.Sockets;

namespace PacketFraming.Client.TestHarness
{
    internal class Program
    {
        const string SERVER_IP = "127.0.0.1";
        const int UDP_PORT = 1234;

        static void Main()
        {
            var udpClient = new UdpClient();

            while (true)
            {
                udpClient.WriteNotificationFrame("127.0.0.1", UDP_PORT, new MyFirstUPDPacket("Hello World"));
                Thread.Sleep(1000);
            }
        }
    }
}
