using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.Shared;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(Core.Engine gameEngine)
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

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 0, 5000), AddFreshEnemiesCallback);

            _gameEngine.Player.Sprite.AddHullHealth(100);
            _gameEngine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(Core.Engine gameEngine, SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Player.ResetAndShow();
            _gameEngine.Events.Create(new System.TimeSpan(0, 0, 0, 0, SiRandom.Between(0, 800)), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(Core.Engine gameEngine, SiEngineCallbackEvent sender, object refObj)
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
            var sprite = _gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            sprite.X = 1600; //This needs to go to the multiplay Multiplay.x,y
            sprite.Y = 600;

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
