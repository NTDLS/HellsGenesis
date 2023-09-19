using HG.Engine;
using HG.Engine.Types;
using System;
using System.Collections.Generic;
using static HG.Engine.Types.HgEngineCallbackEvent;

namespace HG.Situations
{
    internal class _SituationBase
    {
        protected EngineCore _core;
        protected List<HgEngineCallbackEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public HgSituationState State { get; protected set; } = HgSituationState.NotStarted;

        public _SituationBase(EngineCore core, string name)
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

            State = HgSituationState.Ended;
        }

        public virtual void BeginSituation()
        {
            State = HgSituationState.Running;
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
