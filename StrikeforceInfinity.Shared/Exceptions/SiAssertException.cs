using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
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
