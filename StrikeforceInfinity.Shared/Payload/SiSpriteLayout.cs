using StrikeforceInfinity.Shared.Messages.Notify;

namespace StrikeforceInfinity.Shared.Payload
{
    /// <summary>
    /// Contains all of the information need to spawn a sprite.
    /// </summary>
    public class SiSpriteLayout
    {
        public uint MultiplayUID { get; set; }
        public string FullTypeName { get; set; } = string.Empty;
        public SiSpriteVector Vector { get; set; } = new();

        //TODO: need to add stuff like hitpoints so the sprites can be spawned for a latecomer to the server.

        public SiSpriteLayout(Type type, uint multiplayUID)
        {
            if (type.FullName == null)
            {
                throw new Exception("The sprite type name cannot be null.");
            }
            FullTypeName = type.FullName;
            MultiplayUID = multiplayUID;
            Vector.MultiplayUID = multiplayUID;
        }

        public SiSpriteLayout()
        {
        }
    }
}
