using System.Collections.Generic;
using System.Threading;

namespace Si.GameEngine.Core.ThreadPooling
{
    public class DelegateThreadPool
    {
        public delegate void ThreadAction();

        public int ThreadCount { get; private set; }

        private readonly List<Thread> _threads = new();
        private readonly Queue<DelegateThreadPoolQueueToken> _actions = new();
        private readonly AutoResetEvent _queueWaitEvent = new(false);
        private bool _keepRunning = false;

        public DelegateThreadPool(int threadCount)
        {
            ThreadCount = threadCount;
            _keepRunning = true;

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(InternalThreadProc);
                _threads.Add(thread);
                thread.Start();
            }
        }

        public DelegateThreadPoolQueueTokenCollection CreateCollection()
        {
            var collection = new DelegateThreadPoolQueueTokenCollection(this);
            return collection;
        }

        public DelegateThreadPoolQueueToken Enqueue(ThreadAction threadAction)
        {
            lock (_actions)
            {
                var queueToken = new DelegateThreadPoolQueueToken(threadAction);
                _actions.Enqueue(queueToken);
                _queueWaitEvent.Set();
                return queueToken;
            }
        }

        public void Stop()
        {
            _keepRunning = false;
            _threads.ForEach(o => o.Join());
        }

        private void InternalThreadProc()
        {
            DelegateThreadPoolQueueToken queueToken;

            while (_keepRunning)
            {
                bool dequeued;

                lock (_actions)
                {
                    dequeued = _actions.TryDequeue(out queueToken);
                }

                if (dequeued)
                {
                    if (queueToken == null)
                    {
                    }
                    else
                    {
                        queueToken.ThreadAction();
                        queueToken.SetComplete();
                    }
                }
                _queueWaitEvent.WaitOne(1);
            }
        }
    }
}
