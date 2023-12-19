using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Shared.Messages.Query
{
    /// <summary>
    /// Notification from server to each connection that this is the layout which will be used for the game.
    /// </summary>
    public class SiLayoutDirective : IFramePayloadNotification
    {
        public List<SiSpriteInfo> Sprites { get; set; } = new();
    }
}
