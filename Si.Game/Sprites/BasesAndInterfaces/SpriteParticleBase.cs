using Si.Game.Engine;

namespace Si.Game.Sprites
{
    internal class SpriteParticleBase : SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. These are typically used as parts that float away after explosions/fractures.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        public SpriteParticleBase(EngineCore gameCore, string name = "")
            : base(gameCore, name)
        {
            _gameCore = gameCore;
        }
    }
}
