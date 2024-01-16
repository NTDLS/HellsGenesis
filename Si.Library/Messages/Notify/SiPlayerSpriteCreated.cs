using NTDLS.StreamFraming.Payloads;

namespace Si.Library.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection has successfully received
    /// the new situation layout and is ready to actualy start playing.
    /// </summary>
    public class SiPlayerSpriteCreated : IFramePayloadNotification
    {
        public string SelectedPlayerClass { get; set; }
        public Guid PlayerMultiplayUID { get; set; }

        public SiPlayerSpriteCreated(string selectedPlayerClass, Guid playerMultiplayUID)
        {
            SelectedPlayerClass = selectedPlayerClass;
            PlayerMultiplayUID = playerMultiplayUID;
        }
    }
}
