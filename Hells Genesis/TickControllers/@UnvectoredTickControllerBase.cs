using HG.Engine;

namespace HG.TickControllers
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    internal class UnvectoredTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore Core { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(EngineCore core)
        {
            Core = core;
        }
    }
}
