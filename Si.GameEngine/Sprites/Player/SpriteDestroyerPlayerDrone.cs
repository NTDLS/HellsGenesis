using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDestroyerPlayerDrone : SpriteDestroyerPlayer, ISpriteDrone
    {
        public SpriteDestroyerPlayerDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}
