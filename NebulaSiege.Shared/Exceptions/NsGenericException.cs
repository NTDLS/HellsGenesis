using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsGenericException : NsExceptionBase
    {
        public NsGenericException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsGenericException(string? message)
            : base($"Generic exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}