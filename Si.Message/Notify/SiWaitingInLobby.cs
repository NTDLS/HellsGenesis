using NTDLS.ReliableMessaging;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection is now waiting in the lobby for the server to start the game.
    /// </summary>
    public class SiWaitingInLobby : IRmNotification
    {
        public SiWaitingInLobby()
        {
        }
    }
}
