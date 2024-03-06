using Si.Engine.Levels;
using Si.Engine.Situations._Superclass;

namespace Si.Engine.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is a peaceful situation.
    /// </summary>
    internal class SituationFreeFlight : SituationBase
    {
        public SituationFreeFlight(EngineCore engine)
            : base(engine,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            Levels.Add(new LevelFreeFlight(engine));
        }
    }
}
