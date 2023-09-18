using HG.Engine;

namespace HG.Sprites.BaseClasses
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
