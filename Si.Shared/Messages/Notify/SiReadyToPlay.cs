using NTDLS.StreamFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection has successfully received
    /// the new situation layout and is ready to actualy start playing.
    /// </summary>
    public class SiReadyToPlay : IFramePayloadNotification
    {
    }
}
