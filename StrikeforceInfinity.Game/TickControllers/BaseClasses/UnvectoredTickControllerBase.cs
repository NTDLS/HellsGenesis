﻿using StrikeforceInfinity.Game.Engine;

namespace StrikeforceInfinity.Game.TickControllers.BaseClasses
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    internal class UnvectoredTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore GameCore { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(EngineCore gameCore)
        {
            GameCore = gameCore;
        }
    }
}
