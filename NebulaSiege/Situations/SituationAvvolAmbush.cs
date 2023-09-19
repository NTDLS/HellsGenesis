using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Sprites.Enemies;
using NebulaSiege.Sprites.Enemies.Peons;
using NebulaSiege.Utility;
using System.Collections.Generic;

namespace NebulaSiege.Situations
{
    internal class SituationPhoenixAmbush : _SituationBase
    {
        public SituationPhoenixAmbush(EngineCore core)
            : base(core, "Phoenix Ambush")
        {
            TotalWaves = 5;
        }

        readonly List<NsEngineCallbackEvent> events = new List<NsEngineCallbackEvent>();

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Player.Sprite.AddHullHealth(100);
            _core.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            if (_core.Sprites.OfType<_SpriteEnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    EndSituation();
                    return;
                }

                int enemyCount = HgRandom.Generator.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore core, NsEngineCallbackEvent sender, object refObj)
        {
            _core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
        }
    }
}
