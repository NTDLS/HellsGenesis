using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.Library.Mathematics.Geometry;
using System.Linq;

namespace Si.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelSerfFormations : LevelBase
    {
        private bool _waitingOnPopulation = false;

        public LevelSerfFormations(EngineCore engine)
            : base(engine,
                  "Serf Formations",
                  "They fly in formation, which look like easy targets...."
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            AddRecuringFireEvent(1, AdvanceWaveCallback);
            AddRecuringFireEvent(5, RedirectFormationCallback);

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void RedirectFormationCallback(SiDefermentEvent sender, object refObj)
        {
            var formationSerfs = _engine.Sprites.Enemies.VisibleOfType<SpriteEnemySerf>()
                .Where(o => o.Mode == SpriteEnemySerf.AIMode.InFormation).ToList();

            if (formationSerfs.Count > 0)
            {
                if (formationSerfs.Exists(o => o.IsWithinCurrentScaledScreenBounds == true) == false)
                {
                    float angleToPlayer = formationSerfs.First().AngleTo360(_engine.Player.Sprite);

                    foreach (SpriteEnemySerf enemy in formationSerfs)
                    {
                        enemy.Velocity.ForwardAngle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(SiDefermentEvent sender, object refObj)
        {
            _engine.Player.ResetAndShow();
        }

        private void AdvanceWaveCallback(SiDefermentEvent sender, object refObj)
        {
            if (_engine.Sprites.OfType<SpriteEnemyBase>().Count == 0 && !_waitingOnPopulation)
            {
                if (CurrentWave == TotalWaves && _waitingOnPopulation != true)
                {
                    End();
                    return;
                }

                _waitingOnPopulation = true;
                AddSingleFireEvent(5, AddFreshEnemiesCallback);
                CurrentWave++;
            }
        }

        private void AddFreshEnemiesCallback(SiDefermentEvent sender, object refObj)
        {
            SiPoint baseLocation = _engine.Display.RandomOffScreenLocation();
            CreateTriangleFormation(baseLocation, 100 - (CurrentWave + 1) * 10, CurrentWave + 2);
            _engine.Audio.RadarBlipsSound.Play();
            _waitingOnPopulation = false;
        }

        private SpriteEnemySerf AddOneEnemyAt(float x, float y, float angle)
        {
            var enemy = _engine.Sprites.Enemies.Create<SpriteEnemySerf>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ForwardMomentium = 0.8f;
            enemy.Velocity.MaximumSpeed = 6;
            enemy.Velocity.ForwardAngle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(SiPoint baseLocation, float spacing, int depth)
        {
            float angle = SiPoint.AngleTo360(baseLocation, _engine.Player.Sprite);

            for (int col = 0; col < depth; col++)
            {
                for (int row = 0; row < depth - col; row++)
                {
                    AddOneEnemyAt(baseLocation.X + col * spacing,
                        baseLocation.Y + row * spacing + col * spacing / 2,
                        angle);
                }
            }
        }
    }
}
