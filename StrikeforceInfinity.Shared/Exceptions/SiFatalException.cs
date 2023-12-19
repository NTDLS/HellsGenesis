using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiFatalException : SiExceptionBase
    {
        public SiFatalException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiFatalException(string? message)
            : base($"Fatal exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}