using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// The host is telling the server that the level has started and all connections should now show the player drones.
    /// </summary>
    public class SiHostStartedLevel : IRmNotification
    {
        public SiHostStartedLevel()
        {
        }
    }
}
