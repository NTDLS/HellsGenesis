using AI2D.GraphicObjects.Enemies;
using AI2D.Types;
using System.Collections.Generic;
using System.Linq;

namespace AI2D.Engine.Scenarios
{
    public class ScenarioIrlenFormations : BaseScenario
    {
        public ScenarioIrlenFormations(Core core)
            : base(core, "Irlen Formations")
        {
            TotalWaves = 5;
        }

        List<EngineCallbackEvent> events = new List<EngineCallbackEvent>();

        public override void Execute()
        {
            State = ScenarioState.Running;

            _core.Actors.HidePlayer();

            _core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);

            //Keep track of recurring events to we can delete them when we are done.
            events.Add(_core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 0, 0),
                AddFreshEnemiesCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring));

            events.Add(_core.Actors.AddNewEngineCallbackEvent(new System.TimeSpan(0, 0, 0, 5),
                RedirectFormationCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring));
        }

        private void Stop()
        {
            foreach (var obj in events)
            {
                obj.ReadyForDeletion = true;
            }

            State = ScenarioState.Ended;
        }

        private void RedirectFormationCallback(Core core, object refObj)
        {
            List<EnemyIrlen> formationIrlens = _core.Actors.VisibleEnemiesOfType<EnemyIrlen>()
                .Where(o => o.Mode == EnemyIrlen.AIMode.InFormation).ToList();

            if (formationIrlens.Count > 0)
            {
                if(formationIrlens.Exists(o=>o.IsOnScreen == true) == false)
                {
                    double angleToPlayer = formationIrlens.First().AngleTo(_core.Actors.Player);

                    foreach (EnemyIrlen enemy in formationIrlens)
                    {
                        enemy.Velocity.Angle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(Core core, object refObj)
        {
            _core.Actors.ResetAndShowPlayer();
        }

        private void AddFreshEnemiesCallback(Core core, object refObj)
        {
            if (_core.Actors.Enemies.Count == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    Stop();
                    return;
                }

                PointD baseLocation = _core.Display.RandomOffScreenLocation();
                CreateTriangleFormation(baseLocation, 40);
                CurrentWave++;
            }
        }

        private EnemyIrlen AddOneEnemyAt(double x, double y, double angle)
        {
            var enemy = _core.Actors.AddNewEnemy<EnemyIrlen>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ThrottlePercentage = 0.8;
            enemy.Velocity.MaxSpeed = 6;
            enemy.Velocity.Angle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(PointD baseLocation, double spacing)
        {
            double angle = Utility.AngleTo(baseLocation, _core.Actors.Player);

            for (int col = 0; col < 8; col++)
            {
                for (int row = 0; row < (8 - col); row++)
                {
                    AddOneEnemyAt(baseLocation.X + (col * spacing),
                        baseLocation.Y + (row * spacing) + (col * spacing / 2),
                        angle);
                }
            }
        }
    }
}
