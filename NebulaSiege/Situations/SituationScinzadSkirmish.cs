using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Sprites.Enemies.BaseClasses;
using NebulaSiege.Sprites.Enemies.Peons;
using NebulaSiege.Utility;

namespace NebulaSiege.Situations
{
    internal class SituationScinzadSkirmish : SituationBase
    {
        public SituationScinzadSkirmish(EngineCore core)
            : base(core,
                  "Scinzad Skirmish",
                  "Its not a skirmish, its a space aged dog fight."
                  )
        {
            TotalWaves = 5;
        }

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
            if (_core.Sprites.OfType<SpriteEnemyBase>().Count == 0)
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
            _core.Sprites.Enemies.Create<SpriteEnemyScinzad>();
        }
    }
}
