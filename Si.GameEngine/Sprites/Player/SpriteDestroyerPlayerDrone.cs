using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDestroyerPlayerDrone : SpriteDestroyerPlayer, ISpriteDrone
    {
        public SpriteDestroyerPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}
