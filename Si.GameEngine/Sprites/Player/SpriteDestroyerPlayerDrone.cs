using Si.Library.Sprite;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteDestroyerPlayerDrone : SpriteDestroyerPlayer, ISpriteDrone
    {
        public SpriteDestroyerPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
