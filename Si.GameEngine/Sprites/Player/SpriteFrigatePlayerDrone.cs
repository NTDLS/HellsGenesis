using Si.GameEngine.Core;
using Si.Library.Sprite;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteFrigatePlayerDrone : SpriteFrigatePlayer, ISpriteDrone
    {
        public SpriteFrigatePlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
