using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Mathematics
{
    public class SiVelocity
    {
        public delegate void ValueChangeEvent(SiVelocity sender);

        public event ValueChangeEvent? OnMomentiumChanged;
        public event ValueChangeEvent? OnBoostChanged;

        public SiAngle LateralAngle { get; set; } = new();
        public SiAngle ForwardAngle { get; set; } = new();

        /// <summary>
        /// The maximum speed that this ship can travel before taking MaximumBoost into account.
        /// </summary>
        public float MaximumSpeed { get; set; }

        /// <summary>
        /// The additional speed that can be temporarily added to the sprites forward momentium.
        /// </summary>
        public float MaximumBoostSpeed { get; set; }

        /// <summary>
        /// The amount of boost availble until it is depleted and requires recharging.
        /// </summary>
        public float AvailableBoost { get; set; }
        public bool IsBoostCoolingDown { get; set; }

        public float _forwardMomentium;
        /// <summary>
        /// Percentage of forward or reverse momentium expressed as a decimal percentage of the Speed.
        /// </summary>
        public float ForwardMomentium
        {
            get => _forwardMomentium;
            set
            {
                _forwardMomentium = value.Clamp(-1, 1);
                OnMomentiumChanged?.Invoke(this);
            }
        }

        public float _lateralMomentium;
        /// <summary>
        /// Percentage of lateral momentium expressed as a decimal percentage of the Speed.
        /// </summary>
        public float LateralMomentium
        {
            get => _lateralMomentium;
            set
            {
                _lateralMomentium = value.Clamp(-1, 1);
                OnMomentiumChanged?.Invoke(this);
            }
        }

        public SiPoint Vector
        {
            get
            {
                return
                    (ForwardAngle * MaximumSpeed * ForwardMomentium + MaximumBoostSpeed * ForwardBoostMomentium)//Forward / Reverse.
                    + (LateralAngle * MaximumSpeed * LateralMomentium); //Left/Right Strafe.
            }
        }

        public float _forwardBoostMomentium;
        /// <summary>
        /// Percentage of forward boost momentium expressed as a decimal percentage of the MaximumBoostSpeed.
        /// </summary>
        public float ForwardBoostMomentium
        {
            get => _forwardBoostMomentium;
            set
            {
                _forwardBoostMomentium = value.Clamp(-1, 1);
                OnBoostChanged?.Invoke(this);
            }
        }
    }
}
