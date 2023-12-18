using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsFatalException : NsExceptionBase
    {
        public NsFatalException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsFatalException(string? message)
            : base($"Fatal exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}