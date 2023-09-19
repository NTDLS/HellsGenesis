using HG.Engine;
using HG.Engine.Types;
using HG.Sprites.Enemies;
using HG.Sprites.Enemies.Peons;
using HG.Utility;

namespace HG.Situations
{
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(EngineCore core)
            : base(core, "Debugging Galore")
        {
            TotalWaves = 100;
        }

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _core.Player.Sprite.AddHullHealth(100);
            _core.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
            _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
        }

        private void AddFreshEnemiesCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            if (_core.Sprites.OfType<SpriteEnemyBase>().Count == 0)
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
                    _core.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
                }

                _core.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore core, HgEngineCallbackEvent sender, object refObj)
        {
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            _core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _core.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_core.Sprites.Debugs.CreateAtCenterScreen();
            _core.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();

            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyDebug>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyPhoenix>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
            //_core.Sprites.Enemies.Create<EnemyRepulsor>();
            //_core.Sprites.Enemies.Create<EnemySpectre>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
            //_core.Sprites.Enemies.Create<EnemyDevastator>();
        }
    }
}
