using Si.GameEngine.Core;
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
        public SituationChallenge(Engine gameCore)
            : base(gameCore,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new LevelPhoenixAmbush(gameCore));
            Levels.Add(new LevelMinnowSkirmish(gameCore));
            Levels.Add(new LevelSerfFormations(gameCore));
            Levels.Add(new LevelFreeFlight(gameCore));
        }
    }
}
