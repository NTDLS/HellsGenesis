using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Situations.BaseClasses;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using System.Linq;

namespace StrikeforceInfinity.Game.Controller
{
    internal class SituationTickController : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly EngineCore _core;
        public SituationBase CurrentSituation { get; private set; }

        public SituationTickController(EngineCore core)
            : base(core)
        {
            _core = core;
        }

        public void Select(string name)
        {
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _core, });
        }

        public bool AdvanceLevel()
        {
            return CurrentSituation?.Advance() ?? false;
        }

        public void End()
        {
            CurrentSituation?.End();
        }
    }
}
