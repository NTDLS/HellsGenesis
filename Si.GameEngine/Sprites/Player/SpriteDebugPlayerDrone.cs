using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDebugPlayerDrone : SpriteDebugPlayer, ISpriteDrone
    {
        public SpriteDebugPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
