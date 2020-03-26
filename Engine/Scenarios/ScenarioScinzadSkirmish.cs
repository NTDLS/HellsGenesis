using AI2D.GraphicObjects.Enemies;
using AI2D.GraphicObjects.PowerUp;

namespace AI2D.Engine.Scenarios
{
    public class ScenarioScinzadSkirmish : BaseScenario
    {
        public ScenarioScinzadSkirmish(Core core)
            : base(core, "Scinzad Skirmish")
        {
            TotalWaves = 5;
        }

        public override void Execute()
        {
            State = ScenarioState.Running;

            _core.Actors.HidePlayer();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);
        }

        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }

        private void AddFreshEnemiesCallback(Core core, object refObj)
        {
            if (_core.Actors.Enemies.Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    Cleanup();
                    return;
                }

                if (_core.Actors.PowerUps.Count < 100)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        _core.Actors.AddNewPowerUp<PowerUpRepair>();
                        _core.Actors.AddNewPowerUp<PowerUpSheild>();
                    }
                }

                _core.Actors.Player.HitPoints += 100;

                int enemyCount = Utility.Random.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, Utility.RandomNumber(0, 800)), AddEnemyCallback);
                }

                _core.Actors.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(Core core, object refObj)
        {
            _core.Actors.AddNewEnemy<EnemyScinzad>();
        }
    }
}
