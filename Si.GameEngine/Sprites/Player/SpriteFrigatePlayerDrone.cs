using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteFrigatePlayerDrone : SpriteFrigatePlayer, ISpriteDrone
    {
        public SpriteFrigatePlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
