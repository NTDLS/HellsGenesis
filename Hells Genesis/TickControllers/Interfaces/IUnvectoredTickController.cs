namespace HG.Controller.Interfaces
{
    /// <summary>
    /// Tick managers that do no use a vector to update their sprites.
    /// </summary>
    internal interface IUnvectoredTickController<T> : ITickController
    {
        public void ExecuteWorldClockTick();
    }
}
