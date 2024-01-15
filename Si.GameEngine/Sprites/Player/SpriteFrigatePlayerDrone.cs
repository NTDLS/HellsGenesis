using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

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
