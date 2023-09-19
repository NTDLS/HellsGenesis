using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;

namespace NebulaSiege.TickControllers
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" sprite.
    /// </summary>
    internal class _PlayerTickControllerBase<T> : _TickControllerBase<T> where T : class
    {
        public EngineCore Core { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual NsPoint ExecuteWorldClockTick() => new NsPoint();

        public _PlayerTickControllerBase(EngineCore core)
        {
            Core = core;
        }
    }
}
