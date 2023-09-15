using HG.Engine;

namespace HG.Actors.BaseClasses
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
