using NTDLS.TightRPC;
using Si.Library.Payload;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tell the server that a client created a sprite. This should be sent to all lobby connections.
    /// </summary>
    public class SiSpriteCreated : ITightRpcNotification
    {
        public SiSpriteLayout? Layout { get; set; }

        public SiSpriteCreated(SiSpriteLayout layout)
        {
            Layout = layout;
        }
    }
}
