namespace NebulaSiege.Client.Payloads
{
    public class NsActionResponseBoolean : NsBaseActionResponse
    {
        public bool Value { get; set; }

        public NsActionResponseBoolean(bool value)
        {
            Value = value;
        }

        public NsActionResponseBoolean()
        {
        }
    }
}
