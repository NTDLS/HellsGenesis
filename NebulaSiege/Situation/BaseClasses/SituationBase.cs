using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Levels.BaseClasses;
using System.Collections.Generic;

namespace NebulaSiege.Situation.BaseClasses
{
    internal class SituationBase
    {
        protected EngineCore _core;
        protected List<NsEngineCallbackEvent> Events = new();

        public LevelBase CurrentLevel { get; protected set; }
        private int _currentLevelIndex = 0;

        public string Name { get; set; }
        public string Description { get; set; }
        public HgSituationState State { get; protected set; } = HgSituationState.NotStarted;

        public List<LevelBase> Levels { get; protected set; } = new();

        public SituationBase(EngineCore core, string name, string description)
        {
            _core = core;
            Name = name;
            Description = description;
        }

        public void ExecuteWorldClockTick()
        {
            if (CurrentLevel?.State == HgSituationState.Ended)
            {
                //if (AdvanceSituation() == false)
                //{
                //Core.Events.QueueTheDoorIsAjar();
                //}
            }
        }

        public void End()
        {
            lock (CurrentLevel)
            {
                foreach (var obj in Levels)
                {
                    obj.End();
                }
            }

            CurrentLevel = null;
            _currentLevelIndex = 0;
        }

        /// <summary>
        /// Returns true of the situation is advanced, returns FALSE if we have have no more situations in the queue.
        /// </summary>
        /// <returns></returns>
        public bool Advance()
        {
            lock (Levels)
            {
                if (_currentLevelIndex < Levels.Count)
                {
                    _core.Player.Hide();
                    CurrentLevel = Levels[_currentLevelIndex];
                    CurrentLevel.Begin();
                    _currentLevelIndex++;
                    return true;
                }
                else
                {
                    CurrentLevel = null;
                    return false;
                }
            }
        }

        /*
        public virtual void End()
        {
            foreach (var obj in Events)
            {
                obj.ReadyForDeletion = true;
            }

            foreach (var obj in Levels)
            {
                obj.End();
            }

            State = HgSituationState.Ended;
        }
        */

        /*
        public virtual void BeginSituation()
        {
            State = HgSituationState.Running;
        }

        protected NsEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _core.Events.Create(timeout, executeCallback, null, HgCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected NsEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            return _core.Events.Create(timeout, executeCallback);
        }
        */
    }
}
