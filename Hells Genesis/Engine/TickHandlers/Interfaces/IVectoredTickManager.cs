using HG.Types.Geometry;

namespace HG.Engine.TickHandlers.Interfaces
{
    /// <summary>
    /// Tick managers which update their actors using the supplied 2D vector.
    /// </summary>
    internal interface IVectoredTickManager : ITickManager
    {
        /// <summary>
        /// </summary>
        /// <param name="displacementVector">The direction and amount of movement that the player has moved in the current tick.</param>
        public void ExecuteWorldClockTick(HgPoint displacementVector);
    }
}
