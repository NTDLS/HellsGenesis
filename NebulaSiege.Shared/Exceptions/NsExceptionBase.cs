using static NebulaSiege.Shared.NsConstants;

namespace NebulaSiege.Shared.Exceptions
{
    public class NsExceptionBase : Exception
    {
        public NsLogSeverity Severity { get; set; }

        public NsExceptionBase()
        {
            Severity = NsLogSeverity.Exception;
        }

        public NsExceptionBase(string? message)
            : base(message)

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}
