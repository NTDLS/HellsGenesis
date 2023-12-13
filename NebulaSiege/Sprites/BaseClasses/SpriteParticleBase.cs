using NebulaSiege.Engine;

namespace NebulaSiege.Sprites
{
    internal class SpriteParticleBase : SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. These are typically used as parts that float away after explosions/fractures.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        public SpriteParticleBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }
    }
}
