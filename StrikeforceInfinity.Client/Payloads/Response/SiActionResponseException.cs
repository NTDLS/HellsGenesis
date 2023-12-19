namespace StrikeforceInfinity.Client.Payloads.Response
{
    public class SiActionResponseException : SiActionResponse
    {
        public SiActionResponseException(Exception ex)
        {
            ExceptionText = ex.Message;
        }

        public SiActionResponseException(string exceptionText)
        {
            ExceptionText = exceptionText;
        }

        public SiActionResponseException()
        {
        }
    }
}
