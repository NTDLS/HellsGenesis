using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteSerpentPlayerDrone : SpriteSerpentPlayer, ISpriteDrone
    {
        public SpriteSerpentPlayerDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}
