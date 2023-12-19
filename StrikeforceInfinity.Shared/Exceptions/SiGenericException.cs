using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiGenericException : SiExceptionBase
    {
        public SiGenericException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public SiGenericException(string? message)
            : base($"Generic exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}