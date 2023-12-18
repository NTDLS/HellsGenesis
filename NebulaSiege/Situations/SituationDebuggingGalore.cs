using NebulaSiege.Engine;
using NebulaSiege.Levels;
using NebulaSiege.Situations.BaseClasses;

namespace NebulaSiege.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(EngineCore core)
            : base(core,
                  "Debugging Galore",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            Levels.Add(new LevelDebuggingGalore(core));
            Levels.Add(new LevelFreeFlight(core));
        }
    }
}
