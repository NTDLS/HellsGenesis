using System.Threading;

namespace Si.GameEngine.Core.ThreadPooling
{
    public class DelegateThreadPoolQueueToken
    {
        public DelegateThreadPool.ThreadAction ThreadAction { get; private set; }
        public bool IsComplete { get; private set; }
        private readonly AutoResetEvent _queueWaitEvent = new(false);

        public object UserObject { get; set; }

        public DelegateThreadPoolQueueToken(DelegateThreadPool.ThreadAction threadAction)
        {
            ThreadAction = threadAction;
        }

        public void SetComplete()
        {
            IsComplete = true;
            _queueWaitEvent.Set();
        }

        public bool WaitForComplete()
        {
            while (IsComplete == false)
            {
                _queueWaitEvent.WaitOne(1);
            }
            return true;
        }
    }
}
