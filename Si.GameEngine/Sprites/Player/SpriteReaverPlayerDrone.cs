using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteReaverPlayerDrone : SpriteReaverPlayer, ISpriteDrone
    {
        public SpriteReaverPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
