using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.Engine.Sprite.Enemy.Starbase;
using Si.Library;

namespace Si.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(EngineCore engine)
            : base(engine,
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

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(SiDefermentEvent sender, object refObj)
        {
            _engine.Player.ResetAndShow();
            AddSingleFireEvent(SiRandom.Between(0, 800), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(SiDefermentEvent sender, object refObj)
        {
            if (_engine.Sprites.OfType<SpriteEnemyBase>().Count == 0)
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

                _engine.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemies()
        {
            for (int i = 0; i < 10; i++)
            {
                _engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            }

            //_engine.Sprites.Debugs.Create(600, 600);

            _engine.Sprites.Enemies.Create<SpriteEnemyStarbaseGarrison>();

            //_engine.Sprites.Enemies.Create<SpriteEnemyStarbaseGarrison>();

            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();

            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_engine.Sprites.Debugs.CreateAtCenterScreen();
            //_engine.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemySpectre>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
        }
    }
}
