namespace StrikeforceInfinity.Client.Payloads.Response
{
    public class SiActionResponsePing : SiActionResponse
    {
        public Guid SessionId { get; set; }
        public DateTime ServerTimeUTC { get; set; }

        public SiActionResponsePing()
        {
        }

        public SiActionResponsePing(Exception ex)
            : base(ex)
        {
        }
    }
}
