using NTDLS.StreamFraming.Payloads;

namespace StrikeforceInfinity.Shared.ServerMessages.Messages
{
    /// <summary>
    /// Tells the server that a client will be playing on a given game host.
    /// </summary>
    public class SiRegisterForGameHost : IFramePayloadNotification
    {
        public Guid GameHostUID { get; set; }

        public SiRegisterForGameHost(Guid gameHostUID)
        {
            GameHostUID = gameHostUID;
        }
    }
}
