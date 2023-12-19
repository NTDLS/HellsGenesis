namespace StrikeforceInfinity.Shared
{
    public static class SiConstants
    {
        public static string FriendlyName = "StrikeforceInfinity";

        public enum NsLogSeverity
        {
            Trace = 0, //Super-verbose, debug-like information.
            Verbose = 1, //General status messages.
            Warning = 2, //Something the user might want to be aware of.
            Exception = 3 //An actual exception has been thrown.
        }
    }
}
