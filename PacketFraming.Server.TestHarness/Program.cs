using NTDLS.UDPPacketFraming;
using NTDLS.UDPPacketFraming.Payloads;
using PacketFraming.Test.Shared;
using System.Net;
using System.Net.Sockets;

namespace PacketFraming.Server.TestHarness
{
    internal class Program
    {
        const int UDP_PORT = 1234;

        static void Main(string[] args)
        {
            LetsTryUDP();
        }

        private static FrameBuffer _frameBuffer = new();

        private static void LetsTryUDP()
        {
            var udpClient = new UdpClient(UDP_PORT);

            var clientEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);

            var thread = new Thread(o =>
            {
                while (true)
                {
                    udpClient.ReadAndProcessFrames(ref clientEndPoint, _frameBuffer, ProcessFrameNotificationCallback);
                }
            });

            thread.Start();
        }

        private static void ProcessFrameNotificationCallback(IUDPPayloadNotification payload)
        {
            if (payload is MyFirstUPDPacket myFirstUPDPacket)
            {
                Console.WriteLine($"{myFirstUPDPacket.Message}->{myFirstUPDPacket.UID}->{myFirstUPDPacket.TimeStamp}");
            }
        }
    }
}
