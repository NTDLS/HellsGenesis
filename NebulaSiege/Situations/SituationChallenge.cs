using NebulaSiege.Engine;
using NebulaSiege.Levels;
using NebulaSiege.Situations.BaseClasses;

namespace NebulaSiege.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is the first ever built challenge situation - it needs to be expanded.
    /// </summary>
    internal class SituationChallenge : SituationBase
    {
        public SituationChallenge(EngineCore core)
            : base(core,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new LevelPhoenixAmbush(core));
            Levels.Add(new LevelScinzadSkirmish(core));
            Levels.Add(new LevelIrlenFormations(core));
            Levels.Add(new LevelFreeFlight(core));
        }
    }
}
