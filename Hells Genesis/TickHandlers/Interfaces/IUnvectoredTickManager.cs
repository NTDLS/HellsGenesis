namespace HG.TickHandlers.Interfaces
{
    /// <summary>
    /// Tick managers that do no use a vector to update their sprites.
    /// </summary>
    internal interface IUnvectoredTickManager : ITickManager
    {
        public void ExecuteWorldClockTick();
    }
}
