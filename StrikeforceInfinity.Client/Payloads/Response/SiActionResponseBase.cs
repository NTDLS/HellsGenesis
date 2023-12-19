namespace StrikeforceInfinity.Client.Payloads.Response
{
    public class SiActionResponseBase
    {
        public bool Success { get; set; } = true;
        public string? ExceptionText { get; set; }

        public SiActionResponseBase()
        {
        }

        public SiActionResponseBase(Exception ex)
        {
            ExceptionText = ex.Message;
            Success = false;
        }
    }
}
