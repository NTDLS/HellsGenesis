using AI2D.GraphicObjects.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Engine.Scenarios
{
    public class ScenarioAvvolAmbush : BaseScenario
    {
        public ScenarioAvvolAmbush(Core core)
            : base(core, "Avvol Ambush")
        {
            Waves = 3;
        }

        List<EngineCallbackEvent> events = new List<EngineCallbackEvent>();

        public override void Execute()
        {
            State = ScenarioState.Running;

            _core.Actors.HidePlayer();

            events.Add(_core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback));

            events.Add(_core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 5000),
                AddFreshEnemiesCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring));
        }

        private void Stop()
        {
            foreach (var obj in events)
            {
                obj.ReadyForDeletion = true;
            }

            State = ScenarioState.Ended;
        }

        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }

        private void AddFreshEnemiesCallback(Core core, object refObj)
        {
            if (_core.Actors.Enemies.Count == 0)
            {
                if (CurrentWave == Waves)
                {
                    Stop();
                    return;
                }

                int enemyCount = Utility.Random.Next(2, 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Actors.AddNewEnemy<EnemyAvvol>();
                }

                CurrentWave++;
            }
        }
    }
}
