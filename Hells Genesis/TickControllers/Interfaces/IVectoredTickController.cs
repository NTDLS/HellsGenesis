using HG.Engine.Types.Geometry;

namespace HG.Controller.Interfaces
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// </summary>
    internal interface IVectoredTickController : ITickController
    {
        /// <summary>
        /// </summary>
        /// <param name="displacementVector">The direction and amount of movement that the player has moved in the current tick.</param>
        public void ExecuteWorldClockTick(HgPoint displacementVector);
    }
}
