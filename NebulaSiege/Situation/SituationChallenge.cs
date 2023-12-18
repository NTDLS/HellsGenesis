using NebulaSiege.Engine;
using NebulaSiege.Levels;
using NebulaSiege.Situation.BaseClasses;

namespace NebulaSiege.Situations
{
    internal class SituationChallenge : SituationBase
    {
        public SituationChallenge(EngineCore core)
            : base(core,
                  "Challengeh",
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
