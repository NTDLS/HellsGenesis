using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Enemies.Peons;
using HG.Engine;
using HG.Engine.Types;
using HG.Situations.BaseClasses;
using HG.Utility;
using System.Collections.Generic;

namespace HG.Situations
{
    internal class SituationPhoenixAmbush : SituationBase
    {
        public SituationPhoenixAmbush(EngineCore core)
            : base(core, "Phoenix Ambush")
        {
            TotalWaves = 5;
        }

        readonly List<HgEngineCallbackEvent> events = new List<HgEngineCallbackEvent>();

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Player.Actor.AddHullHealth(100);
            _core.Player.Actor.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
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

        private void AddEnemyCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Actors.Enemies.Create<EnemyPhoenix>();
        }
    }
}
