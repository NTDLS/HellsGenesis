using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteCruiserPlayerDrone : SpriteCruiserPlayer, ISpriteDrone
    {
        public SpriteCruiserPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
