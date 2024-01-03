using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyUvesDrone : SpriteEnemyUves, ISpriteDrone
    {
        public SpriteEnemyUvesDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}