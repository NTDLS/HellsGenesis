namespace Si.GameEngine.Sprites._Superclass
{
    public class SpriteParticleBase : SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. These are typically used as parts that float away after explosions/fractures.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        public SpriteParticleBase(Core.Engine gameEngine, string name = "")
            : base(gameEngine, name)
        {
            _gameEngine = gameEngine;
        }
    }
}
