using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using System;
using System.Collections.Generic;
using static StrikeforceInfinity.Game.Engine.Types.SiEngineCallbackEvent;

namespace StrikeforceInfinity.Game.Levels.BaseClasses
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelBase
    {
        protected EngineCore _gameCore;
        protected List<SiEngineCallbackEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public HgLevelState State { get; protected set; } = HgLevelState.NotYetStarted;

        public LevelBase(EngineCore gameCore, string name, string description)
        {
            _gameCore = gameCore;
            Name = name;
            Description = description;
        }

        public virtual void End()
        {
            foreach (var obj in Events)
            {
                obj.QueuedForDeletion = true;
            }

            State = HgLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = HgLevelState.Started;
        }

        protected SiEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _gameCore.Events.Create(timeout, executeCallback, null, HgCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected SiEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            return _gameCore.Events.Create(timeout, executeCallback);
        }
    }
}
