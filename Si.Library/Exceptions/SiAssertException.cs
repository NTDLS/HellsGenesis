using static Si.Library.SiConstants;

namespace Si.Library.Exceptions
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
