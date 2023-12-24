using Si.GameEngine.Engine;

namespace Si.GameEngine.TickControllers.BasesAndInterfaces
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore GameCore { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(EngineCore gameCore)
        {
            GameCore = gameCore;
        }
    }
}
