namespace Si.GameEngine.Core.TickControllers._Superclass
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public Engine GameEngine { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(Engine gameEngine)
        {
            GameEngine = gameEngine;
        }
    }
}
