using Si.GameEngine.Engine;
using Si.GameEngine.Engine.Types;
using System;
using System.Collections.Generic;
using static Si.GameEngine.Engine.Types.SiEngineCallbackEvent;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Levels.BasesAndInterfaces
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    public class LevelBase
    {
        protected EngineCore _gameCore;
        protected List<SiEngineCallbackEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public SiLevelState State { get; protected set; } = SiLevelState.NotYetStarted;

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

            State = SiLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = SiLevelState.Started;
        }

        protected SiEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, SiOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _gameCore.Events.Create(timeout, executeCallback, null, SiCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected SiEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, SiOnExecute executeCallback)
        {
            return _gameCore.Events.Create(timeout, executeCallback);
        }
    }
}
