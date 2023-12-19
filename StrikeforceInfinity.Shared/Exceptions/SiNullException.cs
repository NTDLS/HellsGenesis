using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiNullException : SiExceptionBase
    {
        public SiNullException()
        {
            Severity = SiLogSeverity.Warning;
        }

        public SiNullException(string message)
            : base($"Null exception: {message}.")

        {
            Severity = SiLogSeverity.Exception;
        }
    }
}