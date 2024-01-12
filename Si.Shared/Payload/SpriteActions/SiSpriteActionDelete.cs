namespace Si.Shared.Payload.SpriteActions
{
    /// <summary>
    /// Drone needs to be deleted, not exploded.
    /// </summary>
    public class SiSpriteActionDelete : SiSpriteAction
    {
        public SiSpriteActionDelete(Guid playerMultiplayUID)
            : base(playerMultiplayUID)
        {
        }
    }
}
