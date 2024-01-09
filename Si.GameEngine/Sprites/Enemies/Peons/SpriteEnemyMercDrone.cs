using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyMercDrone : SpriteEnemyMerc, ISpriteDrone
    {
        public SpriteEnemyMercDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}