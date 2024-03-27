using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Mathematics
{
    public class SiVelocity
    {
        public delegate void ValueChangeEvent(SiVelocity sender);

        public event ValueChangeEvent? OnVelocityChanged;
        public event ValueChangeEvent? OnBoostChanged;

        /// <summary>
        /// The maximum speed that this object can travel in any direction (not including MaximumSpeedBoost).
        /// </summary>
        public float MaximumSpeed { get; set; }

        /// <summary>
        /// The additional speed that can be temporarily added to the sprites velocity.
        /// </summary>
        public float MaximumBoostSpeed { get; set; }

        /// <summary>
        /// The amount of boost availble until it is depleted and requires recharging.
        /// </summary>
        public float AvailableBoost { get; set; }
        public bool IsBoostCoolingDown { get; set; }

        public SiPoint _velocity = new();
        /// <summary>
        /// Omni-directional velocity with a magnatude of 1. Expressed as a decimal percentage of the MaximumSpeed in any direction.
        /// </summary>
        public SiPoint Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value.Clamp(-1, 1);
                OnVelocityChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// The sumation of the angle, and all velocity (including boost).
        /// Sprite movement is simple: (MovementVector * epoch)
        /// </summary>
        public SiPoint MovementVector => (Velocity * (MaximumSpeed + (MaximumBoostSpeed * SpeedBoostPercentage)));

        public float _speedBoostPercentage;
        /// <summary>
        /// Percentage of speed boost expressed as a decimal percentage of the MaximumBoostSpeed.
        /// </summary>
        public float SpeedBoostPercentage
        {
            get => _speedBoostPercentage;
            set
            {
                _speedBoostPercentage = value.Clamp(-1, 1);
                OnBoostChanged?.Invoke(this);
            }
        }

        public SiVelocity()
        {
        }

        public SiVelocity(SiAngle velocity)
        {
            Velocity = velocity;
        }
    }
}
