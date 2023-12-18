using NebulaSiege.Engine;
using NebulaSiege.Situations.BaseClasses;
using NebulaSiege.TickControllers.BaseClasses;
using NebulaSiege.Utility;
using System.Linq;

namespace NebulaSiege.Controller
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
            var situationTypes = NsReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = NsReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _core, });
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
