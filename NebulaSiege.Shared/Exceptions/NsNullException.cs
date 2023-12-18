using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsNullException : NsExceptionBase
    {
        public NsNullException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsNullException(string message)
            : base($"Null exception: {message}.")

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}