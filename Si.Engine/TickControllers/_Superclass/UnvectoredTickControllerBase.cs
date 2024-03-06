namespace Si.Engine.TickControllers._Superclass
{
    /// <summary>
    /// Tick managers that do not handle sprites or do not use a vector to update their sprites.
    /// Things like Events, Menues, Radar Position Indicators, etc.
    /// </summary>
    public class UnvectoredTickControllerBase<T> : TickControllerBase<T> where T : class
    {
        public EngineCore GameEngine { get; private set; }

        public virtual void ExecuteWorldClockTick() { }

        public UnvectoredTickControllerBase(EngineCore engine)
        {
            GameEngine = engine;
        }
    }
}
