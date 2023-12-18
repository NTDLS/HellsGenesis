namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponsePing : NsActionResponse
    {
        public Guid SessionId { get; set; }
        public DateTime ServerTimeUTC { get; set; }
    }
}
