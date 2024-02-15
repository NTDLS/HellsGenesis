using NTDLS.ReliableMessaging;
using Si.Library.Payload;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Notification from the lobby owner containing the situation layout.
    /// This is then broadcast from the server to each connection.
    /// </summary>
    public class SiSituationLayout : IRmNotification
    {
        public List<SiSpriteLayout> Sprites { get; set; } = new();
    }
}
