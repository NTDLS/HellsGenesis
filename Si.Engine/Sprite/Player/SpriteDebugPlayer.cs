using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteDebugPlayer : SpritePlayerBase
    {
        public SpriteDebugPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Player\Ships\Debug.png");
        }
    }
}
