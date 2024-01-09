using NTDLS.StreamFraming.Payloads;
using Si.Shared.Payload;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tell the server that a client created a sprite. This should be sent to all lobby connections.
    /// </summary>
    public class SiSpriteCreated : IFramePayloadNotification
    {
        public SiSpriteLayout? Layout { get; set; }

        public SiSpriteCreated(SiSpriteLayout layout)
        {
            Layout = layout;
        }
    }
}
