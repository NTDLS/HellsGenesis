using HG.Types;

namespace HG.TickManagers.Interfaces
{
    /// <summary>
    /// Tick managers that do no use a vector to update their actors.
    /// </summary>
    internal interface IUnvectoredTickManager : ITickManager
    {
        public void ExecuteWorldClockTick();
    }
}
