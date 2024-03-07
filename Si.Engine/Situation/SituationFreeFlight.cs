using Si.Engine.Level;
using Si.Engine.Situation._Superclass;

namespace Si.Engine.Situation
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
