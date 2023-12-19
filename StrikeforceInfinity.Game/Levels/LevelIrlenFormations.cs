using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Levels.BaseClasses;
using StrikeforceInfinity.Game.Sprites.Enemies.BaseClasses;
using StrikeforceInfinity.Game.Sprites.Enemies.Peons;
using StrikeforceInfinity.Game.Utility;
using System.Linq;

namespace StrikeforceInfinity.Game.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelIrlenFormations : LevelBase
    {
        public LevelIrlenFormations(EngineCore core)
            : base(core,
                  "Irlen Formations",
                  "They fly in formation, which look like easy targets...."
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 1), AdvanceWaveCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 5), RedirectFormationCallback);

            _core.Player.Sprite.AddHullHealth(100);
            _core.Player.Sprite.AddShieldHealth(10);
        }

        private void RedirectFormationCallback(EngineCore core, SiEngineCallbackEvent sender, object refObj)
        {
            var formationIrlens = _core.Sprites.Enemies.VisibleOfType<SpriteEnemyIrlen>()
                .Where(o => o.Mode == SpriteEnemyIrlen.AIMode.InFormation).ToList();

            if (formationIrlens.Count > 0)
            {
                if (formationIrlens.Exists(o => o.IsWithinCurrentScaledScreenBounds == true) == false)
                {
                    double angleToPlayer = formationIrlens.First().AngleTo360(_core.Player.Sprite);

                    foreach (SpriteEnemyIrlen enemy in formationIrlens)
                    {
                        enemy.Velocity.Angle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(EngineCore core, SiEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        bool waitingOnPopulation = false;

        private void AdvanceWaveCallback(EngineCore core, SiEngineCallbackEvent sender, object refObj)
        {
            if (_core.Sprites.OfType<SpriteEnemyBase>().Count == 0 && !waitingOnPopulation)
            {
                if (CurrentWave == TotalWaves && waitingOnPopulation != true)
                {
                    End();
                    return;
                }

                waitingOnPopulation = true;
                _core.Events.Create(new System.TimeSpan(0, 0, 0, 5), AddFreshEnemiesCallback);
                CurrentWave++;
            }
        }

        private void AddFreshEnemiesCallback(EngineCore core, SiEngineCallbackEvent sender, object refObj)
        {
            SiPoint baseLocation = _core.Display.RandomOffScreenLocation();
            CreateTriangleFormation(baseLocation, 100 - (CurrentWave + 1) * 10, CurrentWave * 5);
            _core.Audio.RadarBlipsSound.Play();
            waitingOnPopulation = false;
        }

        private SpriteEnemyIrlen AddOneEnemyAt(double x, double y, double angle)
        {
            var enemy = _core.Sprites.Enemies.Create<SpriteEnemyIrlen>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ThrottlePercentage = 0.8;
            enemy.Velocity.MaxSpeed = 6;
            enemy.Velocity.Angle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(SiPoint baseLocation, double spacing, int depth)
        {
            double angle = HgMath.AngleTo360(baseLocation, _core.Player.Sprite);

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
