using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Enemies.Peons;
using HG.Engine;
using HG.Situations.BaseClasses;
using HG.Types;
using System.Linq;

namespace HG.Situations
{
    internal class SituationIrlenFormations : SituationBase
    {
        public SituationIrlenFormations(Core core)
            : base(core, "Irlen Formations")
        {
            TotalWaves = 5;
        }

        public override void BeginSituation()
        {
            base.BeginSituation();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 1), AdvanceWaveCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 5), RedirectFormationCallback);

            _core.Player.Actor.AddHitPoints(100);
            _core.Player.Actor.AddShieldPoints(10);
        }

        private void RedirectFormationCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            var formationIrlens = _core.Actors.Enemies.VisibleOfType<EnemyIrlen>()
                .Where(o => o.Mode == EnemyIrlen.AIMode.InFormation).ToList();

            if (formationIrlens.Count > 0)
            {
                if (formationIrlens.Exists(o => o.IsOnScreen == true) == false)
                {
                    double angleToPlayer = formationIrlens.First().AngleTo(_core.Player.Actor);

                    foreach (EnemyIrlen enemy in formationIrlens)
                    {
                        enemy.Velocity.Angle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            _core.Player.ResetAndShow();
        }

        bool waitingOnPopulation = false;

        private void AdvanceWaveCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            if (_core.Actors.OfType<EnemyBase>().Count == 0 && !waitingOnPopulation)
            {
                if (CurrentWave == TotalWaves && waitingOnPopulation != true)
                {
                    EndSituation();
                    return;
                }

                waitingOnPopulation = true;
                _core.Events.Create(new System.TimeSpan(0, 0, 0, 5), AddFreshEnemiesCallback);
                CurrentWave++;
            }
        }

        private void AddFreshEnemiesCallback(Core core, HgEngineCallbackEvent sender, object refObj)
        {
            HgPoint<double> baseLocation = _core.Display.RandomOffScreenLocation();
            CreateTriangleFormation(baseLocation, 100 - (CurrentWave + 1) * 10, CurrentWave);
            _core.Audio.RadarBlipsSound.Play();
            waitingOnPopulation = false;
        }

        private EnemyIrlen AddOneEnemyAt(double x, double y, double angle)
        {
            var enemy = _core.Actors.Enemies.Create<EnemyIrlen>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ThrottlePercentage = 0.8;
            enemy.Velocity.MaxSpeed = 6;
            enemy.Velocity.Angle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(HgPoint<double> baseLocation, double spacing, int depth)
        {
            double angle = HgMath.AngleTo(baseLocation, _core.Player.Actor);

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
