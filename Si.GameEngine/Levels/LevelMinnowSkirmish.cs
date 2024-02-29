using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.Library;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelMinnowSkirmish : LevelBase
    {
        public LevelMinnowSkirmish(GameEngineCore gameEngine)
            : base(gameEngine,
                  "Minnow Skirmish",
                  "Its not just a skirmish, its a space aged dog fight."
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

            _gameEngine.Player.Sprite.AddHullHealth(100);
            _gameEngine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(SiEngineCallbackEvent sender, object refObj)
        {
            if (_gameEngine.Sprites.OfType<SpriteEnemyBase>().Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    End();
                    return;
                }

                int enemyCount = SiRandom.Between(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    AddSingleFireEvent(SiRandom.Between(0, 800), AddEnemyCallback);
                }

                _gameEngine.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(SiEngineCallbackEvent sender, object refObj)
        {
            _gameEngine.Sprites.Enemies.Create<SpriteEnemyMinnow>();
        }
    }
}
