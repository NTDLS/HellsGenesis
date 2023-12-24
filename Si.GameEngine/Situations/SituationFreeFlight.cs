using Si.GameEngine.Engine;
using Si.GameEngine.Levels;
using Si.GameEngine.Situations.BasesAndInterfaces;

namespace Si.GameEngine.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is a peaceful situation.
    /// </summary>
    internal class SituationFreeFlight : SituationBase
    {
        public SituationFreeFlight(EngineCore gameCore)
            : base(gameCore,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            Levels.Add(new LevelFreeFlight(gameCore));
        }
    }
}
