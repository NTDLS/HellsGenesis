using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;

namespace Si.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is just a peaceful free flight.
    /// </summary>
    internal class LevelFreeFlight : LevelBase
    {
        public LevelFreeFlight(EngineCore engine)
            : base(engine,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(SiDefermentEvent sender, object refObj)
        {
            _engine.Player.ResetAndShow();
        }
    }
}
