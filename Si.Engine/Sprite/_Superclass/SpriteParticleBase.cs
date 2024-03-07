using Si.Engine;

namespace Si.GameEngine.Sprite._Superclass
{
    public class SpriteParticleBase : SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. These are typically used as parts that float away after explosions/fractures.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        public SpriteParticleBase(EngineCore engine, string name = "")
            : base(engine, name)
        {
            _engine = engine;
        }
    }
}
