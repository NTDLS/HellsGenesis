using NTDLS.UDPPacketFraming.Payloads;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static NTDLS.UDPPacketFraming.UDPFraming;

namespace NTDLS.UDPPacketFraming
{
    public class UdpMessageManager
    {
        private static Random _random = new Random();
        private bool _keepRunning = false;
        private Thread? _receiveThread = null;
        public UdpClient? Client { get; set; }

        /// <summary>
        /// Starts a new managed UPD "connection" that can send and receive.
        /// </summary>
        /// <param name="listenPort"></param>
        /// <param name="processNotificationCallback"></param>
        public UdpMessageManager(int listenPort, ProcessFrameNotificationCallback processNotificationCallback)
        {
            Client = new UdpClient(listenPort);
            ListenAsync(listenPort, processNotificationCallback);
        }

        /// <summary>
        /// Starts a new managed UPD handler that can send only.
        /// </summary>
        /// <param name="listenPort"></param>
        /// <param name="processNotificationCallback"></param>
        public UdpMessageManager()
        {
            Client = new UdpClient();
        }

        ~UdpMessageManager()
        {
            try { Client?.Close(); } catch { }
            try { Client?.Dispose(); } catch { }

            Client = null;

        }

        public void Shutdown()
        {
            if (_keepRunning)
            {
                try { Client?.Close(); } catch { }
                try { Client?.Dispose(); } catch { }

                Client = null;

                _keepRunning = false;
                _receiveThread?.Join();
            }
        }

        public static int GetRandomUnusedUDPPort(int minPort, int maxPort)
        {
            while (true)
            {
                int port = _random.Next(minPort, maxPort + 1);
                if (IsPortAvailable(port))
                {
                    return port;
                }
            }
        }

        public static bool IsPortAvailable(int port)
        {
            try
            {
                using var udpClient = new UdpClient(port);
                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }

        public void WriteMessage(string hostOrIPAddress, int port, IUDPPayloadNotification payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteNotificationFrame(hostOrIPAddress, port, payload);
        }

        public void WriteMessage(IPAddress ipAddress, int port, IUDPPayloadNotification payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteNotificationFrame(ipAddress, port, payload);
        }

        public void WriteMessage(IPEndPoint endpoint, IUDPPayloadNotification payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteNotificationFrame(endpoint, payload);
        }

        public void WriteBytes(string hostOrIPAddress, int port, byte[] payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteBytesFrame(hostOrIPAddress, port, payload);
        }

        public void WriteBytes(IPAddress ipAddress, int port, byte[] payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteBytesFrame(ipAddress, port, payload);
        }

        public void WriteBytes(IPEndPoint endpoint, byte[] payload)
        {
            if (Client == null) throw new Exception("The UDP client has not been initialized.");
            Client.WriteBytesFrame(endpoint, payload);
        }

        private void ListenAsync(int listenPort, ProcessFrameNotificationCallback processNotificationCallback)
        {
            if (Client == null)
            {
                throw new Exception("The UDP client has not been initialized.");
            }

            FrameBuffer frameBuffer = new();
            var clientEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

            _keepRunning = true;

            _receiveThread = new Thread(o =>
            {
                while (_keepRunning && Client.ReadAndProcessFrames(ref clientEndPoint, frameBuffer, processNotificationCallback))
                {
                    Thread.Sleep(0);
                }
            });

            _receiveThread.Start();
        }
    }
}
