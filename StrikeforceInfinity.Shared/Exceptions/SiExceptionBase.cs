using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiExceptionBase : Exception
    {
        public NsLogSeverity Severity { get; set; }

        public SiExceptionBase()
        {
            Severity = NsLogSeverity.Exception;
        }

        public SiExceptionBase(string? message)
            : base(message)

        {
            Severity = NsLogSeverity.Exception;
        }
    }
}
