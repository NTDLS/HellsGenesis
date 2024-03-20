using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.GameEngine.Sprite.Enemy.Starbase.Garrison;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;
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
            if (_engine.Sprites.OfType<SpriteEnemyBase>().Count() == 0)
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
            for (int i = 0; i < 25; i++)
            {
                _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
            }

            //_engine.Sprites.Debugs.Add(1000, 1000);

            _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();

            AddAsteroidField(8, 8);

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

        public void AddAsteroidField(int rowCount, int colCount)
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    var asteroid = _engine.Sprites.GenericSprites.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 23)}.png");

                    float totalXOffset = -(asteroid.Size.Width * colCount);
                    float totalYOffset = (_engine.Display.TotalCanvasSize.Height + (asteroid.Size.Height * rowCount));

                    asteroid.Location = new SiPoint(totalXOffset + asteroid.Size.Width * col, totalYOffset - asteroid.Size.Height * row);

                    asteroid.TravelAngle.Degrees = SiRandom.Variance(-45, 0.10f);
                    asteroid.Velocity.MaximumSpeed = SiRandom.Variance(asteroid.Velocity.MaximumSpeed, 0.20f);
                    asteroid.Velocity.ForwardVelocity = 1.0f;

                    asteroid.VectorType = ParticleVectorType.UseTravelAngle;
                    asteroid.RotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Right : SiRelativeDirection.Left;
                    asteroid.RotationSpeed = SiRandom.Between(-1f, 1f);

                    asteroid.SetHullHealth(100);
                }
            }
        }
    }
}
