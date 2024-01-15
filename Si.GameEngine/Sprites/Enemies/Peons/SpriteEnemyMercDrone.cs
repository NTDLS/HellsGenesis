using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyMercDrone : SpriteEnemyMerc, ISpriteDrone
    {
        public SpriteEnemyMercDrone(Engine gameCore)
            : base(gameCore)
        {
        }
    }
}