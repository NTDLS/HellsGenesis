using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiNullException : SiExceptionBase
    {
        public SiNullException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiNullException(string message)
            : base($"Null exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}