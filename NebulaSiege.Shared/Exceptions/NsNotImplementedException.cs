using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsNotImplementedException : NsExceptionBase
    {
        public NsNotImplementedException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsNotImplementedException(string message)
            : base($"Not implemented exception: {message}.")

        {
            Severity = NsLogSeverity.Warning;
        }
    }
}