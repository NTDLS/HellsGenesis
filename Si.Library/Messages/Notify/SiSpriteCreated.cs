using NTDLS.ReliableMessaging;
using Si.Library.Payload;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tell the server that a client created a sprite. This should be sent to all lobby connections.
    /// </summary>
    public class SiSpriteCreated : IRmNotification
    {
        public SiSpriteLayout? Layout { get; set; }

        public SiSpriteCreated(SiSpriteLayout layout)
        {
            Layout = layout;
        }
    }
}
