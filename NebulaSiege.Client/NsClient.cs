using NebulaSiege.Client.Management;
using NebulaSiege.Shared;
using NebulaSiege.Shared.Exceptions;
using System.Diagnostics;

namespace NebulaSiege.Client
{
    public class NsClient : IDisposable
    {
        public bool IsConnected => _connection != null;
        public string ClientName { get; private set; }
        public string BaseAddress { get; private set; }
        public TimeSpan Timeout { get; private set; } = new TimeSpan(0, 8, 0, 0, 0);

        public HttpClient Connection
        {
            get
            {
                NsUtility.EnsureNotNull(_connection);
                return _connection;
            }
        }

        public Guid SessionId { get; private set; }
        public NsServerClient Server { get; private set; }

        private object _pingLock = new();
        private HttpClient? _connection = null;
        private Thread? _keepAliveThread = null;

        /// <summary>
        /// This is the process id of the session on the server. This is populated with each call to Client.Server.Ping().
        /// </summary>
        public ulong ServerProcessId { get; internal set; }

        /// <summary>
        /// Connects to the server using a URL.
        /// </summary>
        /// <param name="baseAddress">Base address should be in the form http://host:port/</param>
        public NsClient(string baseAddress, string clientName = "")
        {
            BaseAddress = baseAddress;
            ClientName = clientName;

            if (string.IsNullOrWhiteSpace(ClientName))
            {
                ClientName = Process.GetCurrentProcess().ProcessName;
            }

            Server = new NsServerClient(this);

            Connect();
        }

        /// <summary>
        /// Connects to the server using a URL and a non-default timeout.
        /// </summary>
        /// <param name="baseAddress">Base address should be in the form http://host:port/</param>
        public NsClient(string baseAddress, TimeSpan timeout, string clientName = "")
        {
            BaseAddress = baseAddress;
            ClientName = clientName;
            Timeout = timeout;

            if (string.IsNullOrWhiteSpace(ClientName))
            {
                ClientName = Process.GetCurrentProcess().ProcessName;
            }

            Server = new NsServerClient(this);

            Connect();
        }

        void Connect()
        {
            if (IsConnected)
            {
                throw new NsGenericException("The client is already connected.");
            }

            try
            {
                SessionId = Guid.NewGuid();
                _connection = new HttpClient
                {
                    BaseAddress = new Uri(BaseAddress),
                    Timeout = Timeout
                };

                Server.Ping();

                _keepAliveThread = new Thread(KeepAliveThread);
                _keepAliveThread.Start();
            }
            catch
            {
                if (_connection != null)
                {
                    try
                    {
                        _connection.Dispose();
                    }
                    catch { }
                }

                _connection = null;
                ServerProcessId = 0;
                throw;
            }
        }

        void Disconnect()
        {
            try
            {
                try
                {
                    if (IsConnected)
                    {
                        Server.CloseSession();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _connection = null;
                ServerProcessId = 0;
            }
        }

        private void KeepAliveThread()
        {
            int approximateSleepTimeMs = 1000;

            while (disposed == false)
            {
                for (int sleep = 0; disposed == false && sleep < approximateSleepTimeMs + 10; sleep++)
                {
                    Thread.Sleep(10);
                }

                lock (_pingLock)
                {
                    if (disposed == false)
                    {
                        try
                        {
                            Server.Ping(); //This keeps the connection alive on the server side.
                        }
                        catch { }
                    }
                }
            }
        }

        #region IDisposable.

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_pingLock)
            {
                if (disposed)
                {
                    return;
                }

                if (disposing)
                {
                    if (IsConnected)
                    {
                        try
                        {
                            Disconnect();
                        }
                        catch { }
                    }
                }

                disposed = true;
            }
        }

        #endregion
    }
}
