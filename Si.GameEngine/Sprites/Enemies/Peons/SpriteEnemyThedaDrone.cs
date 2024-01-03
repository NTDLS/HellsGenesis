using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyThedaDrone : SpriteEnemyTheda, ISpriteDrone
    {
        public SpriteEnemyThedaDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}