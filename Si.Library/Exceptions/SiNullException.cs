using static Si.Library.SiConstants;

namespace Si.Library.Exceptions
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