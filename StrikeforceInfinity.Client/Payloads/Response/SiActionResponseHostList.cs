namespace StrikeforceInfinity.Client.Payloads.Response
{
    public class SiActionResponseHostList : SiActionResponse
    {
        public List<SiGameHost> Collection { get; set; } = new();

        public SiActionResponseHostList()
        {
        }

        public SiActionResponseHostList(Exception ex)
            : base(ex)
        {
        }
    }
}
