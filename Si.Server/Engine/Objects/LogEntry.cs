using static Si.Shared.SiConstants;

namespace Si.Server.Engine.Objects
{
    internal class LogEntry
    {
        public DateTime DateTime { get; set; }
        public string? Message { get; set; }
        public Exception? Exception { get; set; }
        public SiLogSeverity? Severity { get; set; }

        public LogEntry()
        {
            DateTime = DateTime.Now;
        }

        public LogEntry(string message)
        {
            DateTime = DateTime.Now;
            Message = message;
        }
    }
}
