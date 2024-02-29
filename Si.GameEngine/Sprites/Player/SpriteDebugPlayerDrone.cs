using Si.GameEngine.Core;
using Si.Library.Sprite;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDebugPlayerDrone : SpriteDebugPlayer, ISpriteDrone
    {
        public SpriteDebugPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
