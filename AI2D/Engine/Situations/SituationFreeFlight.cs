using AI2D.Events;
using System.Collections.Generic;

namespace AI2D.Engine.Situations
{
    internal class SituationFreeFlight : BaseSituation
    {
        public SituationFreeFlight(Core core)
            : base(core, "Free Flight")
        {
            TotalWaves = 5;
        }

        readonly List<EngineCallbackEvent> events = new List<EngineCallbackEvent>();

        public override void Execute()
        {
            State = ScenarioState.Running;

            _core.Actors.HidePlayer();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }
    }
}
