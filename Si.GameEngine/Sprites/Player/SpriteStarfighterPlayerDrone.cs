using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteStarfighterPlayerDrone : SpriteStarfighterPlayer, ISpriteDrone
    {
        public SpriteStarfighterPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
