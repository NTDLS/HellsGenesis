using NebulaSiege.Engine;
using NebulaSiege.Levels;
using NebulaSiege.Situation.BaseClasses;

namespace NebulaSiege.Situations
{

    internal class SituationFreeFlight : SituationBase
    {
        public SituationFreeFlight(EngineCore core)
            : base(core,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            Levels.Add(new LevelFreeFlight(core));
        }
    }
}
