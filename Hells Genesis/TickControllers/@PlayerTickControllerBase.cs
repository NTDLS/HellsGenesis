using HG.Engine;
using HG.Engine.Types.Geometry;

namespace HG.TickControllers
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" sprite.
    /// </summary>
    internal class PlayerTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore Core { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual HgPoint ExecuteWorldClockTick() => new HgPoint();

        public PlayerTickControllerBase(EngineCore core)
        {
            Core = core;
        }
    }
}
