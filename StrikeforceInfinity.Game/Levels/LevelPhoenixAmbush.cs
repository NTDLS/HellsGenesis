using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Levels.BaseClasses;
using StrikeforceInfinity.Game.Sprites.Enemies.BaseClasses;
using StrikeforceInfinity.Game.Sprites.Enemies.Peons;
using StrikeforceInfinity.Game.Utility;

namespace StrikeforceInfinity.Game.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelPhoenixAmbush : LevelBase
    {
        public LevelPhoenixAmbush(EngineCore gameCore)
            : base(gameCore,
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

                int enemyCount = HgRandom.Generator.Next(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    _gameCore.Events.Create(new System.TimeSpan(0, 0, 0, 0, HgRandom.Between(0, 800)), AddEnemyCallback);
                }

                _gameCore.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(EngineCore gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            _gameCore.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
        }
    }
}
