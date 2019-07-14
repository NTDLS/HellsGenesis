using System;
using System.Collections.Generic;
using static AI2D.Engine.EngineCallbackEvent;

namespace AI2D.Engine.Scenarios
{
    public class BaseScenario
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public enum ScenarioState
        {
            NotStarted,
            Running,
            Ended
        }

        protected List<EngineCallbackEvent> Events = new List<EngineCallbackEvent>();


        protected Core _core;
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public ScenarioState State { get; protected set; } = ScenarioState.NotStarted;

        public BaseScenario(Core core, string name)
        {
            _core = core;
            Name = name;
        }

        public virtual void Cleanup()
        {
            foreach (var obj in Events)
            {
                obj.ReadyForDeletion = true;
            }

            State = ScenarioState.Ended;
        }

        public virtual void Execute()
        {
        }


        protected EngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, OnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _core.Actors.AddNewEngineCallbackEvent(timeout,
                executeCallback, null, EngineCallbackEvent.CallbackEventMode.Recurring);

            Events.Add(obj);
            return obj;
        }

        protected EngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, OnExecute executeCallback)
        {
            return _core.Actors.AddNewEngineCallbackEvent(timeout, executeCallback);
        }
    }
}
