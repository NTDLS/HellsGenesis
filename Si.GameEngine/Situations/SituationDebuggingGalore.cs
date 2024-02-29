using Si.GameEngine.Levels;
using Si.GameEngine.Situations._Superclass;

namespace Si.GameEngine.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class SituationDebuggingGalore : SituationBase
    {
        public SituationDebuggingGalore(GameEngineCore gameEngine)
            : base(gameEngine,
                  "Debugging Galore",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            //Levels.Add(new LevelSerfFormations(gameEngine));
            Levels.Add(new LevelDebuggingGalore(gameEngine));
            Levels.Add(new LevelFreeFlight(gameEngine));
        }
    }
}
