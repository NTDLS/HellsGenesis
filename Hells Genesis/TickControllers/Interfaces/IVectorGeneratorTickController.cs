using HG.Engine.Types.Geometry;

namespace HG.Controller.Interfaces
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" sprite.
    /// </summary>
    internal interface IVectorGeneratorTickController<T> : ITickController
    {
        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public HgPoint ExecuteWorldClockTick();
    }
}
