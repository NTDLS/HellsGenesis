using Si.Library.Mathematics;

namespace Si.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick manager that generates offset vectors for the one and only local player sprite.
    /// </summary>
    public class PlayerSpriteTickControllerBase<T> : ITickController<T> where T : class
    {
        public EngineCore Engine { get; private set; }

        /// <summary>
        /// Moves the player and returns the direction and amount of movment which was applied.
        /// </summary>
        /// <returns>Returns the direction and amount of movement that the player has moved in the current tick.</returns>
        public virtual SiVector ExecuteWorldClockTick(float epochTimeepoch) => new();

        public PlayerSpriteTickControllerBase(EngineCore engine)
        {
            Engine = engine;
        }
    }
}
