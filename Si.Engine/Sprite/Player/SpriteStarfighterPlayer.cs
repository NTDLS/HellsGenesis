using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteStarfighterPlayer : SpritePlayerBase
    {
        public SpriteStarfighterPlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@$"Sprites\Player\Ships\Starfighter.png");
        }
    }
}
