using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiInvalidArgumentException : SiExceptionBase
    {
        public SiInvalidArgumentException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiInvalidArgumentException(string message)
            : base($"Invalid argument exception: {message}.")

        {
            Severity = NsLogSeverity.Warning;
        }
    }
}