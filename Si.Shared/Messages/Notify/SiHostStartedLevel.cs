using NTDLS.StreamFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// The host is telling the server that the level has started and all connections should now show the player drones.
    /// </summary>
    public class SiHostStartedLevel : IFramePayloadNotification
    {
        public SiHostStartedLevel()
        {
        }
    }
}
