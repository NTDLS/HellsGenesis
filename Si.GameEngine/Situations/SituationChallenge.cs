using Si.GameEngine.Levels;
using Si.GameEngine.Situations._Superclass;

namespace Si.GameEngine.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is the first ever built challenge situation - it needs to be expanded.
    /// </summary>
    internal class SituationChallenge : SituationBase
    {
        public SituationChallenge(Core.Engine gameEngine)
            : base(gameEngine,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new LevelPhoenixAmbush(gameEngine));
            Levels.Add(new LevelMinnowSkirmish(gameEngine));
            Levels.Add(new LevelSerfFormations(gameEngine));
            Levels.Add(new LevelFreeFlight(gameEngine));
        }
    }
}
