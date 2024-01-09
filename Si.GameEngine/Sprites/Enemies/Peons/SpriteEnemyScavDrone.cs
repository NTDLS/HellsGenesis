using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyScavDrone : SpriteEnemyScav, ISpriteDrone
    {
        public SpriteEnemyScavDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}