using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Levels.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.Enemies.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.Enemies.Peons;
using StrikeforceInfinity.Game.Utility;

namespace StrikeforceInfinity.Game.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(EngineCore gameCore)
            : base(gameCore,
                  "Debugging Galore",
                  "The level is dire, the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            TotalWaves = 100;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _gameCore.Player.Sprite.AddHullHealth(100);
            _gameCore.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(EngineCore gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            _gameCore.Player.ResetAndShow();
            _gameCore.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(EngineCore gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            if (_gameCore.Sprites.OfType<SpriteEnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    End();
                    return;
                }

                //int enemyCount = Utility.Random.Next(CurrentWave + 1, CurrentWave + 5);
                int enemyCount = 1;

                for (int i = 0; i < enemyCount; i++)
                {
                    AddEnemies();
                }

                _gameCore.Audio.RadarBlipsSound.Play();

                CurrentWave++;

                if (_gameCore.Multiplay.PlayMode == HgPlayMode.MutiPlayerHost)
                {
                    _gameCore.Multiplay.BroadcastLevelLayout();
                }
            }
        }

        private void AddEnemies()
        {
            //_gameCore.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameCore.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameCore.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameCore.Sprites.Enemies.Create<EnemyRepulsor>();

            _gameCore.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _gameCore.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _gameCore.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            _gameCore.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_gameCore.Sprites.Debugs.CreateAtCenterScreen();
            //_gameCore.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyDebug>();
            //_gameCore.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameCore.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameCore.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameCore.Sprites.Enemies.Create<EnemyDevastator>();
            //_gameCore.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameCore.Sprites.Enemies.Create<EnemySpectre>();
            //_gameCore.Sprites.Enemies.Create<EnemyDevastator>();
            //_gameCore.Sprites.Enemies.Create<EnemyDevastator>();
        }
    }
}
