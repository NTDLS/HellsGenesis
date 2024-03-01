using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is just a peaceful free flight.
    /// </summary>
    internal class LevelFreeFlight : LevelBase
    {
        public LevelFreeFlight(GameEngineCore gameEngine)
            : base(gameEngine,
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
            _gameEngine.Player.ResetAndShow();
        }
    }
}
