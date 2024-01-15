using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteStarfighterPlayerDrone : SpriteStarfighterPlayer, ISpriteDrone
    {
        public SpriteStarfighterPlayerDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}
