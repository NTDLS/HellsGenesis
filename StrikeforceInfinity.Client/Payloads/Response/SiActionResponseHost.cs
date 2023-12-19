namespace StrikeforceInfinity.Client.Payloads.Response
{
    public class SiActionResponseHost : SiActionResponse
    {
        public SiGameHost GameHost { get; set; } = new();

        public SiActionResponseHost()
        {
        }

        public SiActionResponseHost(Exception ex)
            : base(ex)
        {
        }
    }
}
