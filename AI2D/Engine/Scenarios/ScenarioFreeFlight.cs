using AI2D.Actors.Enemies;
using System.Collections.Generic;

namespace AI2D.Engine.Scenarios
{
    public class ScenarioFreeFlight : BaseScenario
    {
        public ScenarioFreeFlight(Core core)
            : base(core, "Free Flight")
        {
            TotalWaves = 5;
        }

        List<EngineCallbackEvent> events = new List<EngineCallbackEvent>();

        public override void Execute()
        {
            State = ScenarioState.Running;

            _core.Actors.HidePlayer();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }
    }
}
