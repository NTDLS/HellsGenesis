using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the PointingAngle.
        /// </summary>
        public void RotatePointingDirection(float radians)
        {
            Orientation.Rotate(radians);
            RotationChanged();
        }

        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the PointingAngle.
        /// </summary>
        public void RotateMovementVector(float radians)
        {
            MovementVector.Rotate(radians);
            Orientation.Radians = MovementVector.OrientationInRadians();
            RotationChanged();
        }

        /// <summary>
        /// Instantly points a sprite at another by rotating the movement vector and then recalculates the PointingAngle.
        /// </summary>
        public void RotateMovementVector(SiVector toLocationOf)
        {
            var radians = Location.AngleToInSignedRadians(toLocationOf);

            MovementVector.SetDirctionMaintainMagnitude(radians);
            Orientation.Radians = MovementVector.OrientationInRadians();
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        ///     angle (with given tolerance) then recalculates PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SpriteBase obj, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInUnsignedDegrees(obj);

            if (deltaAngle > varianceDegrees)
            {
                if (deltaAngle >= 180)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else
                {
                    RotateMovementVector(+SiMath.DegToRad(rotationAmountDegrees));
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        /// angle (with given tolerance) then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SiVector toLocation, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInUnsignedDegrees(toLocation);

            if (deltaAngle > varianceDegrees)
            {
                if (deltaAngle >= 180)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target angle
        /// (with given tolerance) then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specifid range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(float toDegrees, float rotationAmountDegrees, float tolerance = 10)
        {
            toDegrees = toDegrees.DenormalizeDegrees();

            if (Orientation.Degrees.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the given amount if it is pointing in the given direction then recalculates the PointingAngle.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if the object is not pointing in the given direction.
        public bool RotateMovementVectorIfPointingAt(SpriteBase obj, float rotationAmountDegrees, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (deltaAngle.IsNotBetween(0, varianceDegrees))
            {
                if (deltaAngle >= 0)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }

                RecalculateMovementVector();

                return true;
            }

            return false;
        }

    }
}
