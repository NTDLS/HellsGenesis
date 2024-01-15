using Si.GameEngine.Core;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Situations._Superclass;
using Si.Shared;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class SituationsTickController : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly Engine _gameCore;
        public SituationBase CurrentSituation { get; private set; }

        public SituationsTickController(Engine gameCore)
            : base(gameCore)
        {
            _gameCore = gameCore;
        }

        public void Select(string name)
        {
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _gameCore, });
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
