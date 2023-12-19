using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Levels.BaseClasses;
using System.Collections.Generic;

namespace StrikeforceInfinity.Game.Situations.BaseClasses
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// </summary>
    internal class SituationBase
    {
        protected EngineCore _core;
        protected List<SiEngineCallbackEvent> Events = new();

        public LevelBase CurrentLevel { get; protected set; }
        private int _currentLevelIndex = 0;

        public string Name { get; set; }
        public string Description { get; set; }
        public HgSituationState State { get; protected set; } = HgSituationState.NotYetStarted;

        public List<LevelBase> Levels { get; protected set; } = new();

        public SituationBase(EngineCore core, string name, string description)
        {
            _core = core;
            Name = name;
            Description = description;
            State = HgSituationState.NotYetStarted;
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

            State = HgSituationState.Ended;

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

                    State = HgSituationState.Started;

                    return true;
                }
                else
                {
                    State = HgSituationState.Ended;

                    CurrentLevel = null;
                    return false;
                }
            }
        }
    }
}
