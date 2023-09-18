using HG.Engine;

namespace HG.Sprites.BaseClasses
{
    internal class ActorParticleBase : ActorBase
    {
        public ActorParticleBase(EngineCore core, string name = "")
            : base(core, name)
        {
            _core = core;
        }
    }
}
