using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteSerpentPlayerDrone : SpriteSerpentPlayer, ISpriteDrone
    {
        public SpriteSerpentPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
