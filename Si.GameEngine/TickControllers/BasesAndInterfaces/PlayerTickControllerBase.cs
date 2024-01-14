using Si.GameEngine.Engine;
using Si.Shared.Types.Geometry;

namespace Si.GameEngine.TickControllers.BasesAndInterfaces
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" sprite.
    /// </summary>
    public class PlayerTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore GameCore { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual SiPoint ExecuteWorldClockTick() => new();

        public PlayerTickControllerBase(EngineCore gameCore)
        {
            GameCore = gameCore;
        }
    }
}
