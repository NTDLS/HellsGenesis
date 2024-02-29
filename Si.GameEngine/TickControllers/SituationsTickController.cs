using Si.GameEngine.Core;
using Si.GameEngine.Situations._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library;
using System.Linq;

namespace Si.GameEngine.TickControllers
{
    public class SituationsTickController : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly GameEngineCore _gameEngine;
        public SituationBase CurrentSituation { get; private set; }

        public SituationsTickController(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void Select(string name)
        {
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _gameEngine, });
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
