using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Mathematics
{
    public class SiVelocity
    {
        public delegate void ValueChangeEvent(SiVelocity sender);

        public event ValueChangeEvent? OnThrottleChanged;
        public event ValueChangeEvent? OnBoostChanged;
        public event ValueChangeEvent? OnRecoilChanged;

        public SiAngle Angle { get; set; } = new();
        public float Speed { get; set; }
        public float Boost { get; set; }
        public float AvailableBoost { get; set; }
        public bool BoostRebuilding { get; set; }

        public float _throttlePercentage;
        public float ThrottlePercentage
        {
            get => _throttlePercentage;
            set
            {
                _throttlePercentage = value.Clamp(-1, 1);
                OnThrottleChanged?.Invoke(this);
            }
        }

        public float _boostPercentage;
        public float BoostPercentage
        {
            get => _boostPercentage;
            set
            {
                _boostPercentage = value.Clamp(-1, 1);
                OnBoostChanged?.Invoke(this);
            }
        }
    }
}
