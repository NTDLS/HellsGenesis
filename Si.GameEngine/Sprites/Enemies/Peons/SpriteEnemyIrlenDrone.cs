using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyIrlenDrone : SpriteEnemyIrlen, ISpriteDrone
    {
        public SpriteEnemyIrlenDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}