using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Situations._Superclass;
using Si.Shared;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
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
