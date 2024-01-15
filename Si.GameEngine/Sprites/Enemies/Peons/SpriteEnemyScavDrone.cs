using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyScavDrone : SpriteEnemyScav, ISpriteDrone
    {
        public SpriteEnemyScavDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}