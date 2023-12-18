namespace NebulaSiege.Client.Payloads.Response
{
    public class NsActionResponseHostList : NsActionResponse
    {
        public List<NsGameHost> Collection { get; set; } = new();

        public NsActionResponseHostList()
        {
        }

        public NsActionResponseHostList(Exception ex)
            : base(ex)
        {
        }
    }
}
