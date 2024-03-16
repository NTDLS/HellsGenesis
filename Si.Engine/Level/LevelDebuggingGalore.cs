using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite;
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
                _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
            }

            _engine.Sprites.Debugs.Add(1000, 1000);

            var playMode = new PlayMode()
            {
                Replay = SiAnimationReplayMode.LoopedPlay,
                DeleteSpriteAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };

            int rowCount = 8;
            int colCount = 8;

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    SpriteAnimation ani;

                    float framesPerSecond = SiRandom.Between(20, 60); //These are native 22fps.

                    switch (SiRandom.Between(1, 3))
                    {
                        case 1:
                            ani = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeIce.png", new Size(246, 248), framesPerSecond, playMode);
                            break;
                        case 2:
                            ani = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeStandard.png", new Size(264, 241), framesPerSecond, playMode);
                            break;
                        default:
                            ani = _engine.Sprites.Animations.Add(@"Sprites\Animation\Asteroid\LargeLava.png", new Size(256, 225), framesPerSecond, playMode);
                            break;
                    }

                    float totalXOffset = -(ani.Size.Width * colCount);
                    float totalYOffset = (_engine.Display.TotalCanvasSize.Height + (ani.Size.Height * rowCount));


                    ani.Location = new SiPoint(totalXOffset + ani.Size.Width * col, totalYOffset - ani.Size.Height * row);
                    ani.Velocity.ForwardAngle.Degrees = -45 + SiRandom.Between(-10, 10);
                    ani.Velocity.MaximumSpeed = 2f + SiRandom.Between(-0.5f, 0.5f);
                    ani.Velocity.ForwardVelocity = 1.0f;
                    ani.TakesDamage = true;
                    ani.SetHullHealth(100);
                }
            }

            _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();


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
