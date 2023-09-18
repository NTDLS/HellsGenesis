using HG.Engine;

namespace HG.Sprites
{
    internal class SpriteParticleBase : SpriteBase
    {
        public SpriteParticleBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }
    }
}
