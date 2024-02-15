using NTDLS.DatagramMessaging;
using Si.Library.Payload.SpriteActions;

namespace Si.Library.Messages.Notify.Datagram
{
    /// <summary>
    /// Tells the server that a sprite has done something. Fire, move, explode, etc.
    /// </summary>
    public class SiSpriteActions : IDmNotification
    {
        public Guid ConnectionId { get; set; }
        public List<SiSpriteAction> Collection { get; set; }

        public SiSpriteActions(List<SiSpriteAction> collection)
        {
            Collection = collection;
        }
    }
}
