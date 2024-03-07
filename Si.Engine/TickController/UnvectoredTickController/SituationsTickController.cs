using Si.Engine;
using Si.GameEngine.Situation._Superclass;
using Si.GameEngine.TickController._Superclass;
using Si.Library;
using System.Linq;

namespace Si.GameEngine.TickController.UnvectoredTickController
{
    public class SituationsTickController : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly EngineCore _engine;
        public SituationBase CurrentSituation { get; private set; }

        public SituationsTickController(EngineCore engine)
            : base(engine)
        {
            _engine = engine;
        }

        public void Select(string name)
        {
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _engine, });
        }

        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.CurrentLevel != null)
            {
                if (CurrentSituation.CurrentLevel.State == SiConstants.SiLevelState.Ended)
                {
                    CurrentSituation?.AdvanceLevel();
                }
            }
        }

        public bool AdvanceLevel()
        {
            return CurrentSituation?.AdvanceLevel() ?? false;
        }

        public void End()
        {
            CurrentSituation?.End();
        }
    }
}
