namespace NebulaSiege.Client.Payloads
{
    public class NsBaseActionResponse
    {
        public bool Success { get; set; } = true;
        public string? ExceptionText { get; set; }

        public NsBaseActionResponse()
        {
        }

        public NsBaseActionResponse(Exception ex)
        {
            ExceptionText = ex.Message;
            Success = false;
        }
    }
}
