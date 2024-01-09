using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemySerfDrone : SpriteEnemySerf, ISpriteDrone
    {
        public SpriteEnemySerfDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}