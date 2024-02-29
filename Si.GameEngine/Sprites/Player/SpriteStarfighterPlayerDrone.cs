using Si.GameEngine.Core;
using Si.Library.Sprite;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteStarfighterPlayerDrone : SpriteStarfighterPlayer, ISpriteDrone
    {
        public SpriteStarfighterPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
