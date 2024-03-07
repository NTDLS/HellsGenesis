using Si.Engine;
using Si.GameEngine.Level;
using Si.GameEngine.Situation._Superclass;

namespace Si.GameEngine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(EngineCore engine)
            : base(engine,
                  "Debugging Galore",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            //Levels.Add(new LevelSerfFormations(engine));
            Levels.Add(new LevelDebuggingGalore(engine));
            Levels.Add(new LevelFreeFlight(engine));
        }
    }
}
