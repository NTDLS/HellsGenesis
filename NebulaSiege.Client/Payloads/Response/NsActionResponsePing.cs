namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponsePing : NsActionResponse
    {
        public Guid SessionId { get; set; }
        public DateTime ServerTimeUTC { get; set; }

        public NsActionResponsePing()
        {
        }

        public NsActionResponsePing(Exception ex)
            : base(ex)
        {
        }
    }
}
