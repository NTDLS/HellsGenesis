using Si.Library.Mathematics;
using Si.Library.Sprite;

namespace Si.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase : ISprite
    {
        /// <summary>
        /// Sets the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector() => MovementVector = MakeMovementVector();

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(float angleInRadians) => MovementVector = MakeMovementVector(angleInRadians);

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVector(SiVector angle) => MovementVector = MakeMovementVector(angle);

        /// <summary>
        /// Returns the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector() => Orientation * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(float angleInRadians) => new SiVector(angleInRadians) * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public SiVector MakeMovementVector(SiVector angle) => angle.Normalize() * Speed * Throttle;
    }
}
