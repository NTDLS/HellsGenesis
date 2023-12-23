using static Si.Shared.SiConstants;

namespace Si.Shared.Exceptions
{
    public class SiAssertException : SiExceptionBase
    {
        public SiAssertException()
        {
            Severity = SiLogSeverity.Warning;
        }

        public SiAssertException(string message)
            : base($"Assert exception: {message}.")

        {
            Severity = SiLogSeverity.Exception;
        }
    }
}
