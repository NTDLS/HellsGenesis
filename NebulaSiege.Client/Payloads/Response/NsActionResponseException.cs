namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponseException : NsActionResponse
    {
        public NsActionResponseException(Exception ex)
        {
            ExceptionText = ex.Message;
        }

        public NsActionResponseException(string exceptionText)
        {
            ExceptionText = exceptionText;
        }

        public NsActionResponseException()
        {
        }
    }
}
