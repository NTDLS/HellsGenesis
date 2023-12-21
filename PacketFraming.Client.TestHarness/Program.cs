using NTDLS.UDPPacketFraming;
using PacketFraming.Test.Shared;
using System.Net.Sockets;

namespace PacketFraming.Client.TestHarness
{
    internal class Program
    {
        static void Main()
        {
            var udpManager = new UdpMessageManager();

            int packetNumber = 0;

            while (true)
            {
                udpManager.WriteMessage("127.0.0.1", 1234,
                    new MyFirstUPDPacket($"Packet#:{packetNumber++} "));
                //Thread.Sleep(1000);
            }
        }
    }
}
