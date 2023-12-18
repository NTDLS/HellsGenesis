namespace NebulaSiege.Client.Payloads
{
    public class NsActionResponseInteger : NsBaseActionResponse
    {
        public int Value { get; set; }

        public NsActionResponseInteger(int value)
        {
            Value = value;
        }

        public NsActionResponseInteger()
        {
        }
    }
}
