using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Enemies.Bosses;
using HG.Engine;
using HG.Situations.BaseClasses;

namespace HG.Situations
{
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(Core core)
            : base(core, "Debugging Galore")
        {
            TotalWaves = 100;
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
            _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.RandomNumber(0, 800)), AddEnemyCallback);

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

                //int enemyCount = Utility.Random.Next(CurrentWave + 1, CurrentWave + 5);
                int enemyCount = 1;

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
            _core.Actors.Enemies.Create<EnemyLouse>();
            //_core.Actors.Enemies.Create<EnemyDebug>();
            //_core.Actors.Enemies.Create<EnemyAvvol>();
            //_core.Actors.Enemies.Create<EnemyScarab>();


            //TODO **************** Point ↑ at ↓ ***************************** in a tick controller.
            //Maybe evewn add a debug tick controller???


            //debug.X = 500;
            //debug.Y = 500;
        }
    }
}
