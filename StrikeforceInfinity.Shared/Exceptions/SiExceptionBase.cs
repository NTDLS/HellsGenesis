using static StrikeforceInfinity.Shared.SiConstants;

namespace StrikeforceInfinity.Shared.Exceptions
{
    public class SiExceptionBase : Exception
    {
        public SiLogSeverity Severity { get; set; }

        public SiExceptionBase()
        {
            Severity = SiLogSeverity.Exception;
        }

        public SiExceptionBase(string? message)
            : base(message)

        {
            Severity = SiLogSeverity.Exception;
        }
    }
}
