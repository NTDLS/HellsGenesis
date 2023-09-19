using HG.Engine;

namespace HG.Sprites
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
