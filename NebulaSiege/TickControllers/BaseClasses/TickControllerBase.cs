namespace NebulaSiege.TickControllers.BaseClasses
{
    /// <summary>
    /// All tick-controllers are derived from this class. They control the "movement of time" for all objects..
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface TickControllerBase<T> where T : class
    {
    }
}
