namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponse : NsActionResponseBase
    {
        public NsActionResponse()
        {
        }

        public NsActionResponse(Exception ex)
            : base(ex)
        {
        }
    }
}

