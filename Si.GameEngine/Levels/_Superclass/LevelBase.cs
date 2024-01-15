using Si.GameEngine.Core.Types;
using System;
using System.Collections.Generic;
using static Si.GameEngine.Core.Types.SiEngineCallbackEvent;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Levels._Superclass
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    public class LevelBase
    {
        protected Core.Engine _gameEngine;
        protected List<SiEngineCallbackEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public SiLevelState State { get; protected set; } = SiLevelState.NotYetStarted;

        public LevelBase(Core.Engine gameEngine, string name, string description)
        {
            _gameEngine = gameEngine;
            Name = name;
            Description = description;
        }

        public virtual void End()
        {
            foreach (var obj in Events)
            {
                obj.QueuedForDeletion = true;
            }

            State = SiLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = SiLevelState.Started;
            _gameEngine.Multiplay.NotifyLevelStarted();
        }

        protected SiEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, SiOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _gameEngine.Events.Create(timeout, executeCallback, null, SiCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected SiEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, SiOnExecute executeCallback)
        {
            return _gameEngine.Events.Create(timeout, executeCallback);
        }
    }
}
