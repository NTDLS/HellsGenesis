using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyMinnowDrone : SpriteEnemyMinnow, ISpriteDrone
    {
        public SpriteEnemyMinnowDrone(EngineCore gameCore)
             : base(gameCore)
        {
        }
    }
}