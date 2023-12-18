using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsInvalidArgumentException : NsExceptionBase
    {
        public NsInvalidArgumentException()
        {
            Severity = NsLogSeverity.Warning;
        }

        public NsInvalidArgumentException(string message)
            : base($"Invalid argument exception: {message}.")

        {
            Severity = NsLogSeverity.Warning;
        }
    }
}