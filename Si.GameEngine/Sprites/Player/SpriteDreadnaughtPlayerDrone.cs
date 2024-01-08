using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDreadnaughtPlayerDrone : SpriteDreadnaughtPlayer, ISpriteDrone
    {
        public SpriteDreadnaughtPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
