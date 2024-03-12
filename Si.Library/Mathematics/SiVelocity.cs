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
        /// The angle in which the object is pointing.
        /// </summary>
        public SiAngle ForwardAngle { get; set; } = new();

        /// <summary>
        /// The maximum speed that this ship can travel before taking MaximumBoost into account.
        /// </summary>
        public float MaximumSpeed { get; set; }

        /// <summary>
        /// The additional speed that can be temporarily added to the sprites forward velocity.
        /// </summary>
        public float MaximumSpeedBoost { get; set; }

        /// <summary>
        /// The amount of boost availble until it is depleted and requires recharging.
        /// </summary>
        public float AvailableBoost { get; set; }
        public bool IsBoostCoolingDown { get; set; }

        public float _forwardVelocity;
        /// <summary>
        /// Percentage of forward or reverse velocity expressed as a decimal percentage of the MaximumSpeed.
        /// </summary>
        public float ForwardVelocity
        {
            get => _forwardVelocity;
            set
            {
                _forwardVelocity = value.Clamp(-1, 1);
                OnVelocityChanged?.Invoke(this);
            }
        }

        public float _lateralVelocity;
        /// <summary>
        /// Percentage of lateral velocity expressed as a decimal percentage of the MaximumSpeed.
        /// </summary>
        public float LateralVelocity
        {
            get => _lateralVelocity;
            set
            {
                _lateralVelocity = value.Clamp(-1, 1);
                OnVelocityChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// The sumation of the forward angle, laterial strafe angle, and all velocity (including boost).
        /// Sprite movement is simple: (MovementVector * epoch)
        /// </summary>
        public SiPoint MovementVector
        {
            get
            {
                /// 90 degrees to the left of the forward angle, so negative LateralVelocity is left and positive LateralVelocity is right.
                var lateralAngle = new SiAngle(ForwardAngle.Radians + SiPoint.RADIANS_90);

                return
                    (ForwardAngle * ((MaximumSpeed * ForwardVelocity) + (MaximumSpeedBoost * ForwardBoostVelocity))) //Forward / Reverse.
                    + (lateralAngle * MaximumSpeed * LateralVelocity); //Left/Right Strafe.
            }
        }

        public float _forwardBoostVelocity;
        /// <summary>
        /// Percentage of forward boost velocity expressed as a decimal percentage of the MaximumBoostSpeed.
        /// </summary>
        public float ForwardBoostVelocity
        {
            get => _forwardBoostVelocity;
            set
            {
                _forwardBoostVelocity = value.Clamp(-1, 1);
                OnBoostChanged?.Invoke(this);
            }
        }
    }
}
