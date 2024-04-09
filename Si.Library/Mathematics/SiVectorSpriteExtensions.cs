using Si.Library.Sprite;
using System.Runtime.CompilerServices;

namespace Si.Library.Mathematics
{
    public static class SiVectorSpriteExtensions
    {
        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in signed degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 1-180 to -1-180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this ISprite fromSprite, ISprite toSprite)
        {
            var angle = fromSprite.Location.AngleToInUnsignedDegrees(toSprite.Location);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in unsigned degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in signed radians.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0 though +π and -π though 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedRadians(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInSignedRadians(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another sprite location in unsigned radians.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0 though 2*π.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedRadians(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.AngleToInUnsignedRadians(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to sprite location from 1-180 to -1-180.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-180 to -180-0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInSignedDegrees(this ISprite fromSprite, SiVector toLocation)
        {
            var angle = fromSprite.Location.AngleToInUnsignedDegrees(toLocation);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Calculates the angle of one location to a sprite location.
        /// </summary>
        /// <param name="fromLocation">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this SiVector fromLocation, ISprite toSprite)
            => fromLocation.AngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Calculates the angle of one sprite location to another location from 0 - 360.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The point to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleToInUnsignedDegrees(this ISprite fromSprite, SiVector toLocation)
            => fromSprite.Location.AngleToInUnsignedDegrees(toLocation);

        ///
        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAway(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInUnsignedDegrees(atSprite));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the sprite is pointing AWAY another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <param name="maxDistance"></param>
        /// <returns>True if the object is pointing away from the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAway(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees, float maxDistance)
            => fromSprite.IsPointingAway(atSprite, toleranceDegrees) && fromSprite.DistanceTo(atSprite) <= maxDistance;

        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees"></param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInSignedDegrees(atSprite));
            return deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees;
        }

        /// <summary>
        /// Returns true if the sprite is pointing AT another sprite, taking into account the tolerance in degrees and max distance.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="atSprite">The object to which the calculation is based.</param>
        /// <param name="toleranceDegrees">The angle in degrees to consider the object to pointing at the other.</param>
        /// <param name="maxDistance">The distance in consider the object to pointing at the other.</param>
        /// <returns>True if the object is pointing at the other given the constraints.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPointingAt(this ISprite fromSprite, ISprite atSprite, float toleranceDegrees, float maxDistance)
        {
            var deltaAngle = Math.Abs(fromSprite.HeadingAngleToInUnsignedDegrees(atSprite));
            if (deltaAngle < toleranceDegrees || deltaAngle > 360 - toleranceDegrees)
            {
                return fromSprite.DistanceTo(atSprite) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// Returns the angle which would be requird to rotate a sprite to be pointing at another sprite.
        /// positive figures indicate right (starboard) side and negative indicate left-hand (port) side of the object.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInSignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.HeadingAngleToInSignedDegrees(toSprite.Location);

        /// <summary>
        /// Returns the angle which would be requird to rotate a sprite to to be pointing at a given location.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 180--180.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInSignedDegrees(this ISprite fromSprite, SiVector toLocation)
        {
            var angle = fromSprite.HeadingAngleToInUnsignedDegrees(toLocation);
            if (angle > 180)
            {
                angle -= 180;
                angle = 180 - angle;
                angle *= -1;
            }

            return -angle;
        }

        /// <summary>
        /// Returns the angle which would be requird to rotate a sprite to be pointing at another sprite.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInUnsignedDegrees(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.HeadingAngleToInUnsignedDegrees(toSprite.Location);

        /// <summary>
        /// Returns the angle which would be requird to rotate a sprite to be pointing at a given location.
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toLocation">The location to which the calculation is based.</param>
        /// <returns>The calculated angle in the range of 0-360.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HeadingAngleToInUnsignedDegrees(this ISprite fromSprite, SiVector toLocation)
        {
            float fromAngle = fromSprite.Orientation.DegreesUnsigned;

            float angleTo = fromSprite.AngleToInUnsignedDegrees(toLocation);

            if (fromAngle < 0) fromAngle = 0 - fromAngle;
            if (angleTo < 0)
            {
                angleTo = 0 - angleTo;
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0f - Math.Abs(angleTo) % 360.0f;
            }

            return angleTo;
        }

        /// <summary>
        /// Returns the distance from one sprite to another
        /// </summary>
        /// <param name="fromSprite">The object from which the calcualtion is based.</param>
        /// <param name="toSprite">The object to which the calculation is based.</param>
        /// <returns>The calcuated distance from one object to the other.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this ISprite fromSprite, ISprite toSprite)
            => fromSprite.Location.DistanceTo(toSprite.Location);
    }
}
