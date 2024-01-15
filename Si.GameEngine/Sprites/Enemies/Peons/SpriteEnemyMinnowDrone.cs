using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyMinnowDrone : SpriteEnemyMinnow, ISpriteDrone
    {
        public SpriteEnemyMinnowDrone(Engine gameCore)
             : base(gameCore)
        {
        }
    }
}