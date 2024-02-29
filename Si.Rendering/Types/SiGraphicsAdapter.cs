namespace Si.Rendering.Types
{
    public class SiGraphicsAdapter
    {
        public int DeviceId { get; set; }
        public string Description { get; set; }

        public double VideoMemoryMb { get; set; }

        public SiGraphicsAdapter(int deviceId, string description)
        {
            DeviceId = deviceId;
            Description = description;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
