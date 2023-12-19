using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server that a client will be playing on a given lobby.
    /// </summary>
    public class SiRegisterToLobby : IFramePayloadNotification
    {
        public Guid LobbyUID { get; set; }

        public SiRegisterToLobby(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
        }
    }
}
