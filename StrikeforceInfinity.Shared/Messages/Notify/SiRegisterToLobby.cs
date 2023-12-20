using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.Messages.Notify
{
    /// <summary>
    /// Tell the server that a connection has selected the lobby. This does not mean
    /// that they have selected a loadout yet. That is denoted with a call to SetWaitingInLobby().
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
