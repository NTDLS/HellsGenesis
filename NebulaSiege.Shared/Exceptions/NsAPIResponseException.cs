using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsAPIResponseException : NsExceptionBase
    {
        public NsAPIResponseException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsAPIResponseException(string? message)
            : base($"API exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}