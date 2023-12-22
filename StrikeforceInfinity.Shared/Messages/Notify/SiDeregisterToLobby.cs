using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Notify
{
    /// <summary>
    /// Tell the server that a connection is leaving the lobby entirely.
    /// </summary>
    public class SiDeregisterToLobby : IFramePayloadNotification
    {
        public Guid LobbyUID { get; set; }

        public SiDeregisterToLobby(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
