using Si.Game.Engine;
using Si.Game.Engine.Types;
using Si.Game.Levels.BasesAndInterfaces;
using Si.Game.Sprites.Enemies.BasesAndInterfaces;
using Si.Game.Sprites.Enemies.Peons;
using Si.Game.Utility;

namespace Si.Game.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelScinzadSkirmish : LevelBase
    {
        public LevelScinzadSkirmish(EngineCore gameCore)
            : base(gameCore,
                  "Scinzad Skirmish",
                  "Its not a skirmish, its a space aged dog fight."
                  )
        {
            TotalWaves = 5;
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

                int enemyCount = SiRandom.Generator.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _gameCore.Events.Create(new System.TimeSpan(0, 0, 0, 0, SiRandom.Between(0, 800)), AddEnemyCallback);
                }

                _gameCore.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            _gameCore.Sprites.Enemies.Create<SpriteEnemyScinzad>();
        }
    }
}
