namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponseBase
    {
        public bool Success { get; set; } = true;
        public string? ExceptionText { get; set; }

        public NsActionResponseBase()
        {
        }

        public NsActionResponseBase(Exception ex)
        {
            ExceptionText = ex.Message;
            Success = false;
        }
    }
}
