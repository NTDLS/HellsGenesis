using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteReaverPlayer : SpritePlayerBase
    {
        public SpriteReaverPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Player\Ships\Reaver.png");
        }
    }
}
