using Si.Shared.Payload.DroneActions;

namespace Si.Shared.Payload
{
    /// <summary>
    /// Contains all of the information need to spawn a sprite.
    /// </summary>
    public class SiSpriteLayout
    {
        public Guid MultiplayUID { get; set; }
        public string FullTypeName { get; set; } = string.Empty;
        public SiSpriteVector Vector { get; set; } = new();

        //TODO: need to add stuff like hitpoints so the sprites can be spawned for a latecomer to the server.

        public SiSpriteLayout(string fullTypeName, Guid multiplayUID)
        {
            FullTypeName = fullTypeName;
            MultiplayUID = multiplayUID;
            Vector.MultiplayUID = multiplayUID;
        }

        public SiSpriteLayout()
        {
        }
    }
}
