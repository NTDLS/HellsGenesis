using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Levels;
using NebulaSiege.Game.Situations.BaseClasses;

namespace NebulaSiege.Game.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is a peaceful situation.
    /// </summary>
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
