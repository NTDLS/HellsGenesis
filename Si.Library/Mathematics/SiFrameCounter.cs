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
        public float AverageFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Average());
        public float MinimumFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Min());
        public float MaximumFrameRate => SiUtility.Interlock(_samples, (obj) => obj.Items.Max());

        public SiFrameCounter()
        {
            _stopwatch.Start();
        }

        public void Calculate()
        {
            // Calculate elapsed time in seconds.
            var elapsedSeconds = (double)_stopwatch.ElapsedTicks / Stopwatch.Frequency;

            CurrentFrameRate = (float)(1.0 / elapsedSeconds);

            _samples.Push(CurrentFrameRate);
            _stopwatch.Restart();
        }
    }
}
