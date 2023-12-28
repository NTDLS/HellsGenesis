using NTDLS.StreamFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection is now waiting in the lobby for the server to start the game.
    /// </summary>
    public class SiWaitingInLobby : IFramePayloadNotification
    {
        public SiWaitingInLobby()
        {
        }
    }
}
