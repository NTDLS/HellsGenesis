using NTDLS.UDPPacketFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server where exactly a sprite is, which direction they are facing and how fast they are going.
    /// </summary>
    public class SiSpriteVectors : IUDPPayloadNotification
    {
        public Guid ConnectionId { get; set; }
        public List<SiSpriteVector> Collection { get; set; }
        public SiSpriteVectors(List<SiSpriteVector> collection)
        {
            Collection = collection;
        }
    }
}
