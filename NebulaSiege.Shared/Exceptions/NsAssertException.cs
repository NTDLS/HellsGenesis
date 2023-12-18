using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsAssertException : NsExceptionBase
    {
        public NsAssertException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsAssertException(string message)
            : base($"Assert exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}