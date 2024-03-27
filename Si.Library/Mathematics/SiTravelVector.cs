using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Mathematics
{
    public class SiTravelVector
    {
        public delegate void ValueChangeEvent(SiTravelVector sender);

        public event ValueChangeEvent? OnVelocityChanged;

        /// <summary>
        /// The speed that this object can generally travel in any direction.
        /// </summary>
        public float Speed { get; set; }

        public SiPoint _velocity = new();
        /// <summary>
        /// Omni-directional velocity.
        /// </summary>
        public SiPoint Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                OnVelocityChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// The sumation of the angle and all velocity .
        /// Sprite movement is simple: (MovementVector * epoch)
        /// </summary>
        public SiPoint MovementVector => Velocity * ThrottlePercentage;

        public float _throttlePercentage = 1.0f;
        /// <summary>
        /// Percentage of speed expressed as a decimal percentage from 0 to 2.
        /// </summary>
        public float ThrottlePercentage
        {
            get => _throttlePercentage;
            set
            {
                _throttlePercentage = value.Clamp(0, 2);
            }
        }

        public SiTravelVector()
        {
        }

        public SiTravelVector(SiAngle velocity)
        {
            Velocity = velocity;
        }
    }
}
