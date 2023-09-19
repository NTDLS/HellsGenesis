using NebulaSiege.Engine;

namespace NebulaSiege.Sprites
{
    internal class _SpriteParticleBase : _SpriteBase
    {
        public _SpriteParticleBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }
    }
}
