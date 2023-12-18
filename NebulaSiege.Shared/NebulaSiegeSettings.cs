namespace NebulaSiege.Shared
{
    public class NebulaSiegeSettings
    {
        /// <summary>
        /// The base listening URL for the web-services.
        /// </summary>
        public string BaseAddress { get; set; } = string.Empty;

        /// <summary>
        /// Causes the server to write super-verbose information about almost every internal operation.
        /// </summary>
        public bool WriteTraceData { get; set; }

        /// <summary>
        /// If true, text logs will be flused at every write. This ensures that the log file is always up-to-date on disk.
        /// </summary>
        public bool FlushLog { get; set; }

        /// <summary>
        /// The directory where text and performance logs are stores.
        /// </summary>
        public string LogDirectory
        {
            get => logDirectory;
            set => logDirectory = value.TrimEnd(['/', '\\']).Trim();
        }
        private string logDirectory = string.Empty;

        /// <summary>
        /// The number of seconds that a connection must be idle before it is automatically closed.
        /// </summary>
        public int MaxIdleConnectionSeconds { get; set; }
    }
}
