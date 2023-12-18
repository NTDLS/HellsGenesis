namespace NebulaSiege.Client.Payloads
{
    public class NsActionResponseUInt : NsBaseActionResponse
    {
        public uint Value { get; set; }

        public NsActionResponseUInt(uint value)
        {
            Value = value;
        }

        public NsActionResponseUInt()
        {
        }
    }
}
