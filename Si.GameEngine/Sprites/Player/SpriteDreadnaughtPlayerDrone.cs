using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDreadnaughtPlayerDrone : SpriteDreadnaughtPlayer, ISpriteDrone
    {
        public SpriteDreadnaughtPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
