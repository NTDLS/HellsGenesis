using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Levels;
using StrikeforceInfinity.Game.Situations.BasesAndInterfaces;

namespace StrikeforceInfinity.Game.Situations
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is the first ever built challenge situation - it needs to be expanded.
    /// </summary>
    internal class SituationChallenge : SituationBase
    {
        public SituationChallenge(EngineCore gameCore)
            : base(gameCore,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new LevelPhoenixAmbush(gameCore));
            Levels.Add(new LevelScinzadSkirmish(gameCore));
            Levels.Add(new LevelIrlenFormations(gameCore));
            Levels.Add(new LevelFreeFlight(gameCore));
        }
    }
}
