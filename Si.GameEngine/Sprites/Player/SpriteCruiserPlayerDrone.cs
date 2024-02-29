using Si.Library.Sprite;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteCruiserPlayerDrone : SpriteCruiserPlayer, ISpriteDrone
    {
        public SpriteCruiserPlayerDrone(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }
    }
}
