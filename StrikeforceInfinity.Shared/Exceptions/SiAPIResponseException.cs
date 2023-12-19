using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiAPIResponseException : SiExceptionBase
    {
        public SiAPIResponseException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiAPIResponseException(string? message)
            : base($"API exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}