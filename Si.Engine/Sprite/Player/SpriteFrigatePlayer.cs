using Si.Engine.Sprite.Player._Superclass;

namespace Si.Engine.Sprite.Player
{
    internal class SpriteFrigatePlayer : SpritePlayerBase
    {
        public SpriteFrigatePlayer(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@$"Sprites\Player\Ships\Frigate.png");
        }
    }
}
