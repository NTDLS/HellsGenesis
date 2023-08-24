﻿using HG.Types;

namespace HG.TickManagers.Interfaces
{
    /// <summary>
    /// Tick managers that generate offset vectors. Realistically, this is only the "player" actor.
    /// </summary>
    internal interface IVectorGeneratorTickManager : ITickManager
    {
        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public HgPoint<double> ExecuteWorldClockTick();
    }
}
