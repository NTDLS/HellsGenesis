using Si.GameEngine.Engine;
using Si.Shared.Messages.Notify;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyPhoenixDrone : SpriteEnemyPhoenix, ISpriteDrone
    {
        public SpriteEnemyPhoenixDrone(EngineCore gameCore)
            : base(gameCore)
        {
        }
    }
}