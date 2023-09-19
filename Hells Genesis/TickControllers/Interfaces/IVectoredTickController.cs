using HG.Engine.Types.Geometry;
using System.Collections.Generic;

namespace HG.Controller.Interfaces
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// </summary>
    internal interface IVectoredTickController<T> : ITickController
    {
        /// <summary>
        /// </summary>
        /// <param name="displacementVector">The direction and amount of movement that the player has moved in the current tick.</param>
        public void ExecuteWorldClockTick(HgPoint displacementVector);


        public List<subType> VisibleOfType<subType>() where subType : T;
        public List<T> Visible();
        public List<T> All();
        public List<subType> OfType<subType>() where subType : T;
    }
}
