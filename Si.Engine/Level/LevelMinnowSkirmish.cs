using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.Library;

namespace Si.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelMinnowSkirmish : LevelBase
    {
        public LevelMinnowSkirmish(EngineCore engine)
            : base(engine,
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

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(SiDefermentEvent sender, object refObj)
        {
            _engine.Player.ResetAndShow();
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

                int enemyCount = SiRandom.Between(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    AddSingleFireEvent(SiRandom.Between(0, 800), AddEnemyCallback);
                }

                _engine.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(SiDefermentEvent sender, object refObj)
        {
            _engine.Sprites.Enemies.Create<SpriteEnemyMinnow>();
        }
    }
}
