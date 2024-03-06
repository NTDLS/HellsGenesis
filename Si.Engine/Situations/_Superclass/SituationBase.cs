using Si.Engine.Core.Types;
using Si.Engine.Levels._Superclass;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Situations._Superclass
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// </summary>
    public class SituationBase
    {
        protected EngineCore _engine;
        protected List<SiDefermentEvent> Events = new();

        public LevelBase CurrentLevel { get; protected set; }
        private int _currentLevelIndex = 0;

        public string Name { get; set; }
        public string Description { get; set; }
        public SiSituationState State { get; protected set; } = SiSituationState.NotYetStarted;

        public List<LevelBase> Levels { get; protected set; } = new();

        public SituationBase(EngineCore engine, string name, string description)
        {
            _engine = engine;
            Name = name;
            Description = description;
            State = SiSituationState.NotYetStarted;
        }

        public void End()
        {
            if (CurrentLevel != null)
            {
                lock (CurrentLevel)
                {
                    foreach (var obj in Levels)
                    {
                        obj.End();
                    }
                }

                State = SiSituationState.Ended;

                CurrentLevel = null;
                _currentLevelIndex = 0;
            }
        }

        /// <summary>
        /// Returns true of the situation is advanced, returns FALSE if we have have no more situations in the queue.
        /// </summary>
        /// <returns></returns>
        public bool AdvanceLevel()
        {
            lock (Levels)
            {
                if (_currentLevelIndex < Levels.Count)
                {
                    _engine.Player.Hide();
                    CurrentLevel = Levels[_currentLevelIndex];
                    CurrentLevel.Begin();
                    _currentLevelIndex++;

                    State = SiSituationState.Started;

                    return true;
                }
                else
                {
                    State = SiSituationState.Ended;

                    CurrentLevel = null;
                    return false;
                }
            }
        }
    }
}
