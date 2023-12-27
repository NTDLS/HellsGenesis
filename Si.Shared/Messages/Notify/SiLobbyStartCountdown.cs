using NTDLS.StreamFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// The server is telling all of the people waiting in the lobby that the game is about to start.
    /// </summary>
    public class SiLobbyStartCountdown : IFramePayloadNotification
    {
        public Guid LobbyUID { get; set; }
        public int SecondsRemaining { get; set; }

        public SiLobbyStartCountdown(Guid lobbyUID, int secondsRemaining)
        {
            LobbyUID = lobbyUID;
            SecondsRemaining = secondsRemaining;
        }
    }
}
