using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteReaverPlayerDrone : SpriteReaverPlayer, ISpriteDrone
    {
        public SpriteReaverPlayerDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}
