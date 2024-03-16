using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.GameEngine.Sprite.Enemy.Starbase.Garrison;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Drawing;
using static Si.Engine.Sprite.SpriteAnimation;
using static Si.Library.SiConstants;

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
            //AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

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
                //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
            }

            //_engine.Sprites.Debugs.Add(1000, 1000);

            var playMode = new PlayMode()
            {
                Replay = SiAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            var ani1 = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeIce.png", new Size(246, 248), 22, playMode);
            ani1.CenterInUniverse();
            ani1.Location -= new SiPoint(256, 0);
            //ani1.Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 360);
            //ani1.Velocity.MaximumSpeed = 0.5f;
            //ani1.Velocity.ForwardVelocity = 1f;
            ani1.TakesDamage = true;
            ani1.SetHullHealth(100000);
            /*
            var ani2 = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeLava.png", new Size(256, 225), 22, playMode);
            ani2.CenterInUniverse();
            //ani2.Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 360);
            //ani2.Velocity.MaximumSpeed = 0.5f;
            //ani2.Velocity.ForwardVelocity = 1f;
            ani2.TakesDamage = true;
            ani2.SetHullHealth(100000);

            var ani3 = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeStandard.png", new Size(264, 241), 22, playMode);
            ani3.CenterInUniverse();
            ani3.Location += new SiPoint(256, 0);
            //ani3.Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 360);
            //ani3.Velocity.MaximumSpeed = 0.5f;
            //ani3.Velocity.ForwardVelocity = 1f;
            ani3.TakesDamage = true;
            ani3.SetHullHealth(100000);
            */
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();

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
