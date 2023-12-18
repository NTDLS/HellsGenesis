namespace NebulaSiege.Client.Payloads
{
    public class NsActionResponseGuid : NsBaseActionResponse
    {
        public Guid Id { get; set; }

        public NsActionResponseGuid(Guid id)
        {
            Id = id;
        }

        public NsActionResponseGuid()
        {
        }
    }
}
