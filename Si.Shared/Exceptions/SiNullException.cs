using static Si.Shared.SiConstants;

namespace Si.Shared.Exceptions
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