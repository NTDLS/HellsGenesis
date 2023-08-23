namespace HG.Types
{
    internal class HGVelocity<T>
    {
        public delegate void ThrottleChange(HGVelocity<T> sender);
        public event ThrottleChange OnThrottleChange;

        public HGAngle<T> Angle { get; set; } = new HGAngle<T>();
        public T MaxSpeed { get; set; }
        public T MaxBoost { get; set; }
        public T AvailableBoost { get; set; }
        public T MaxRotationSpeed { get; set; }

        public T _throttlePercentage;
        public T ThrottlePercentage
        {
            get
            {
                return _throttlePercentage;
            }
            set
            {
                _throttlePercentage = value;
                _throttlePercentage = (dynamic)_throttlePercentage > 1 ? 1 : (dynamic)_throttlePercentage;
                _throttlePercentage = (dynamic)_throttlePercentage < -1 ? -1 : (dynamic)_throttlePercentage;

                OnThrottleChange?.Invoke(this);
            }
        }


        public T _boostPercentage;
        public T BoostPercentage
        {
            get
            {
                return _boostPercentage;
            }
            set
            {
                _boostPercentage = value;
                _boostPercentage = (dynamic)_boostPercentage > 1 ? 1 : (dynamic)_boostPercentage;
                _boostPercentage = (dynamic)_boostPercentage < -1 ? -1 : (dynamic)_boostPercentage;
            }
        }
    }
}
