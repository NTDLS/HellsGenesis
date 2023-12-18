using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Server.Logging
{
    public class LogEntry
    {
        public DateTime DateTime { get; set; }
        public string? Message { get; set; }
        public Exception? Exception { get; set; }
        public NsLogSeverity? Severity { get; set; }

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
