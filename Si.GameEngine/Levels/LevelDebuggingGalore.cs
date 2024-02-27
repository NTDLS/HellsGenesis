using Si.GameEngine.Core;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.Library;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(GameEngineCore gameEngine)
            : base(gameEngine,
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

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

            _gameEngine.Player.Sprite.AddHullHealth(100);
            _gameEngine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(GameEngineCore gameEngine, SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Player.ResetAndShow();
            AddSingleFireEvent(SiRandom.Between(0, 800), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(GameEngineCore gameEngine, SiEngineCallbackEvent sender, object refObj)
        {
            if (_gameEngine.Sprites.OfType<SpriteEnemyBase>().Count == 0)
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

                _gameEngine.Audio.RadarBlipsSound.Play();

                CurrentWave++;

                _gameEngine.Multiplay.BroadcastSituationLayout();
            }
        }

        private void AddEnemies()
        {
            for (int i = 0; i < 10; i++)
            {
                _gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            }

            _gameEngine.Sprites.Debugs.Create(600, 600);

            //_gameEngine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameEngine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameEngine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameEngine.Sprites.Enemies.Create<EnemyRepulsor>();

            //_gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_gameEngine.Sprites.Debugs.CreateAtCenterScreen();
            //_gameEngine.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDebug>();
            //_gameEngine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameEngine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameEngine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDevastator>();
            //_gameEngine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_gameEngine.Sprites.Enemies.Create<EnemySpectre>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDevastator>();
            //_gameEngine.Sprites.Enemies.Create<EnemyDevastator>();
        }
    }
}
