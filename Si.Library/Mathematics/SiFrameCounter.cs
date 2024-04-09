using System.Diagnostics;

namespace Si.Library.Mathematics
{
    /// <summary>
    /// Used to keep track of the FPS that the world clock is executing at.
    /// </summary>
    public class SiFrameCounter
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly CircularBuffer<float> _samples = new(1000);

        public float CurrentFrameRate { get; private set; }
        public float ElapsedMilliseconds => (float)(((double)_stopwatch.ElapsedTicks / Stopwatch.Frequency) * 1000.0);
        public float ElapsedMicroseconds => (float)(_stopwatch.ElapsedTicks * 1000000.0 / Stopwatch.Frequency);

        public float AverageFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Average());
        public float MinimumFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Min());
        public float MaximumFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Max());

        public SiFrameCounter()
        {
            _stopwatch.Start();
        }

        public void Calculate()
        {
            CurrentFrameRate = (float)(1000.0f / ElapsedMilliseconds);
            _stopwatch.Restart();
            _samples.Push(CurrentFrameRate);
        }
    }
}
