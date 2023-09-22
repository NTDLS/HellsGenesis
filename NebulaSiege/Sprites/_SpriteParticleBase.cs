using NebulaSiege.Engine;

namespace NebulaSiege.Sprites
{
    internal class _SpriteParticleBase : _SpriteBase
    {
        /// <summary>
        /// Used to represent a particle sprite. These are typically used as parts that float away after explosions/fractures.
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        public _SpriteParticleBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }
    }
}
