using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.Shared;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelPhoenixAmbush : LevelBase
    {
        public LevelPhoenixAmbush(Core.Engine gameEngine)
            : base(gameEngine,
                  "Phoenix Ambush",
                  "We're safe now - or are we? Its an AMBUSH!"
                  )
        {
            TotalWaves = 5;
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

                int enemyCount = SiRandom.Generator.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _gameEngine.Events.Create(new System.TimeSpan(0, 0, 0, 0, SiRandom.Between(0, 800)), AddEnemyCallback);
                }

                _gameEngine.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(Core.Engine gameEngine, SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
        }
    }
}
