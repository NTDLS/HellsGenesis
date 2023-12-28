using NTDLS.StreamFraming.Payloads;

namespace Si.Shared.Messages.Notify
{
    /// <summary>
    /// Tells the server that the connection is now waiting in the lobby for the server to start the game.
    /// </summary>
    public class SiWaitingInLobby : IFramePayloadNotification
    {
        /// <summary>
        /// The player class that the player selected.
        /// </summary>
        public Type SelectedClass { get; set; }

        /// <summary>
        /// The MultiplayUID for the player sprite at the connection that created the sprite. This will be broadcast to all connections
        /// and they are exprected to create the same sprite with the same MultiplayUID.
        /// </summary>
        public Guid PlayerMultiplayUID { get; set; }

        public SiWaitingInLobby(Type selectedClass, Guid playerMultiplayUID)
        {
            PlayerMultiplayUID = playerMultiplayUID;
            SelectedClass = selectedClass;
        }
    }
}
