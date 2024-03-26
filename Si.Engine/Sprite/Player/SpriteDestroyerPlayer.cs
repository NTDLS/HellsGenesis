using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteDestroyerPlayer : SpritePlayerBase
    {
        public SpriteDestroyerPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Player\Ships\Destroyer.png");
        }
    }
}
