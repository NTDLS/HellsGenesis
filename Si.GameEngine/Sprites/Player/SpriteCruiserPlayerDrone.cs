using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteCruiserPlayerDrone : SpriteCruiserPlayer, ISpriteDrone
    {
        public SpriteCruiserPlayerDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}
