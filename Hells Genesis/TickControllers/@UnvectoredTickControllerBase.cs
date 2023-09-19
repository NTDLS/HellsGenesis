using HG.Engine;

namespace HG.TickControllers
{
    /// <summary>
    /// Tick managers that do no use a vector to update their sprites.
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
