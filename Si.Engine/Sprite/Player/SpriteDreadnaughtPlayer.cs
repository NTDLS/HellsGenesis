using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteDreadnaughtPlayer : SpritePlayerBase
    {
        public SpriteDreadnaughtPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Player\Ships\Dreadnaught.png");
        }
    }
}
