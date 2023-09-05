using HG.Engine;
using System;
using System.Collections.Generic;
using static HG.Engine.HgEngineCallbackEvent;

namespace HG.Situations.BaseClasses
{
    internal class SituationBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public enum ScenarioState
        {
            NotStarted,
            Running,
            Ended
        }

        protected List<HgEngineCallbackEvent> Events = new();

        protected Core _core;
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public ScenarioState State { get; protected set; } = ScenarioState.NotStarted;

        public SituationBase(Core core, string name)
        {
            _core = core;
            Name = name;
        }

        public virtual void EndSituation()
        {
            foreach (var obj in Events)
            {
                obj.ReadyForDeletion = true;
            }

            State = ScenarioState.Ended;
        }

        public virtual void BeginSituation()
        {
            State = ScenarioState.Running;
        }

        protected HgEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _core.Events.Create(timeout, executeCallback, null, HgCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected HgEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            return _core.Events.Create(timeout, executeCallback);
        }
    }
}
