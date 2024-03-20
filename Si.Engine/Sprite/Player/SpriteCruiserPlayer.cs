using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteCruiserPlayer : SpritePlayerBase
    {
        public SpriteCruiserPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Player\Ships\Cruiser.png");
        }
    }
}
