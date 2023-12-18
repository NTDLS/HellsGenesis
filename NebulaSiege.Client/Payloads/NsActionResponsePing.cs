namespace NebulaSiege.Client.Payloads
{
    public class NsActionResponsePing : NsBaseActionResponse
    {
        public Guid SessionId { get; set; }
        public ulong ProcessId { get; set; }
        public DateTime ServerTimeUTC { get; set; }
    }
}
