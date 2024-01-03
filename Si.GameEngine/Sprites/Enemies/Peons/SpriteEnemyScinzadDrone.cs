using Si.GameEngine.Engine;
using Si.Sprites.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Peons
{
    internal class SpriteEnemyScinzadDrone : SpriteEnemyScinzad, ISpriteDrone
    {
        public SpriteEnemyScinzadDrone(EngineCore gameCore)
             : base(gameCore)
        {
        }
    }
}