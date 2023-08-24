using HG.Actors.Objects.Enemies;

namespace HG.Engine.Situations
{
    internal class SituationScinzadSkirmish : BaseSituation
    {
        public SituationScinzadSkirmish(Core core)
            : base(core, "Scinzad Skirmish")
        {
            TotalWaves = 5;
        }

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Actors.Player.AddHitPoints(100);
            _core.Actors.Player.AddShieldPoints(10);
        }

        private void FirstShowPlayerCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }

        private void AddFreshEnemiesCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            if (_core.Actors.OfType<EnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    EndSituation();
                    return;
                }

                int enemyCount = HGRandom.Random.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HGRandom.RandomNumber(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            _core.Actors.Enemies.Create<EnemyScinzad>();
        }
    }
}
