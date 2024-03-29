using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Drawing;

namespace Si.GameEngine.Sprite.SupportingClasses
{
    /// <summary>
    /// Contains the sprite and the bounds that it is predicted to occupy after ApplyMotion().
    /// Keep in mind that this is rudimentary in the way that it predicts the next location but decisively so.
    /// </summary>
    public class PredictedSpriteRegion
    {
        /// <summary>
        /// Reference to the sprite.
        /// </summary>
        public SpriteInteractiveBase Sprite { get; set; }

        public SiPoint RenderWindowPosition { get; private set; }

        /// <summary>
        /// Size of the referenced sprite.
        /// </summary>
        public Size Size => Sprite.Size;

        /// <summary>
        /// Predicted location after next call to ApplyMotion().
        /// </summary>
        public SiPoint Location { get; private set; }

        /// <summary>
        /// Predicted velocity after next call to ApplyMotion().
        /// </summary>
        public SiPoint Velocity = new();

        /// <summary>
        /// Predicted direction after next call to ApplyMotion().
        /// </summary>
        public SiAngle Direction { get; private set; }

        /// <summary>
        /// Predicted bounds after next call to ApplyMotion().
        /// </summary>
        public RectangleF Bounds => new(Location.X - Size.Width / 2.0f, Location.Y - Size.Height / 2.0f, Size.Width, Size.Height);

        public virtual RawRectangleF RawRenderBounds => new(
                        (RenderLocation.X - Size.Width / 2.0f),
                        (RenderLocation.Y - Size.Height / 2.0f),
                        (RenderLocation.X - Size.Width / 2.0f) + Size.Width,
                        (RenderLocation.Y - Size.Height / 2.0f) + Size.Height);

        public SiPoint RenderLocation => Location - RenderWindowPosition;

        public PredictedSpriteRegion(SpriteInteractiveBase sprite, SiPoint renderWindowPosition, float epoch)
        {
            RenderWindowPosition = renderWindowPosition;

            Sprite = sprite;

            //Assume the sprite is using rotation.
            Direction = new SiAngle(sprite.Direction.Radians + sprite.RotationSpeed * epoch);

            //Assuming the sprite is moving in the direction it is pointing.
            Velocity = Direction * Velocity.Length();

            //Determine the sprites new location
            Location = sprite.Location + Velocity * sprite.Throttle * epoch;
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsAABB(PredictedSpriteRegion otherObject) =>
            Bounds.IntersectsWith(otherObject.Bounds);

        /// <summary>
        /// Determines if two (non-axis-aligned) rectangles interset using Separating Axis Theorem (SAT).
        /// This allows us to determine if a rotated rectangle interescts another rotated rectangle.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsSAT(PredictedSpriteRegion otherObject)
            => SiSeparatingAxisTheorem.IntersectsRotated(Bounds, Direction.Radians,
                otherObject.Bounds, otherObject.Direction.Radians);
    }
}
