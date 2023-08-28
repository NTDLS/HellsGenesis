using HG.Actors.Enemies;
using HG.Actors.Enemies.Peons;
using HG.Engine;

namespace HG.Situations
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

            _core.Player.Actor.AddHitPoints(100);
            _core.Player.Actor.AddShieldPoints(10);
        }

        private void FirstShowPlayerCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            if (_core.Actors.OfType<EnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    EndSituation();
                    return;
                }

                int enemyCount = HgRandom.Random.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.RandomNumber(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Actors.Enemies.Create<EnemyScinzad>();
        }
    }
}
