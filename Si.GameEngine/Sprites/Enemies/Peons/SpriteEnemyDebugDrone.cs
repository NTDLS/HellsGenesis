using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyDebugDrone : SpriteEnemyDebug, ISpriteDrone
    {
        public SpriteEnemyDebugDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}