using NebulaSiege.Game.Engine;

namespace NebulaSiege.Game.Sprites
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
