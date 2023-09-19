using NebulaSiege.Engine;

namespace NebulaSiege.TickControllers
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    internal class _UnvectoredTickControllerBase<T> : _TickControllerBase<T> where T : class
    {
        public EngineCore Core { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public _UnvectoredTickControllerBase(EngineCore core)
        {
            Core = core;
        }
    }
}
