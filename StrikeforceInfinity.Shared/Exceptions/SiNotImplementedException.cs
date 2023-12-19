using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiNotImplementedException : SiExceptionBase
    {
        public SiNotImplementedException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiNotImplementedException(string message)
            : base($"Not implemented exception: {message}.")

        {
            Severity = NsLogSeverity.Warning;
        }
    }
}