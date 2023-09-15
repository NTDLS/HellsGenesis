using HG.Engine.Types.Geometry;
using HG.Utility.ExtensionMethods;

namespace HG.Engine.Types
{
    internal class HgVelocity
    {
        public delegate void ValueChangeEvent(HgVelocity sender);

        public event ValueChangeEvent OnThrottleChanged;
        public event ValueChangeEvent OnBoostChanged;
        public event ValueChangeEvent OnRecoilChanged;

        public HgAngle Angle { get; set; } = new();
        public double MaxSpeed { get; set; }
        public double MaxBoost { get; set; }
        public double AvailableBoost { get; set; }
        public bool BoostRebuilding { get; set; }

        private double _recoilPercentage = 0;
        public double RecoilPercentage
        {
            get => _recoilPercentage;
            set
            {
                _recoilPercentage = value.Box(0, 1);
                OnRecoilChanged?.Invoke(this);
            }
        }

        public double _throttlePercentage;
        public double ThrottlePercentage
        {
            get => _throttlePercentage;
            set
            {
                _throttlePercentage = value.Box(-1, 1);
                OnThrottleChanged?.Invoke(this);
            }
        }

        public double _boostPercentage;
        public double BoostPercentage
        {
            get => _boostPercentage;
            set
            {
                _boostPercentage = value.Box(-1, 1);
                OnBoostChanged?.Invoke(this);
            }
        }
    }
}
