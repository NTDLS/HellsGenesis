using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteSerpentPlayer : SpritePlayerBase
    {
        public SpriteSerpentPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@$"Sprites\Player\Ships\Serpent.png");
        }
    }
}
