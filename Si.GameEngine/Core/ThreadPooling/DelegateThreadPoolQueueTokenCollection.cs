using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Si.GameEngine.Core.ThreadPooling.DelegateThreadPool;

namespace Si.GameEngine.Core.ThreadPooling
{
    public class DelegateThreadPoolQueueTokenCollection
    {
        public List<DelegateThreadPoolQueueToken> Collection { get; private set; } = new();
        private readonly DelegateThreadPool _threadPool;

        public DelegateThreadPoolQueueTokenCollection(DelegateThreadPool threadPool)
        {
            _threadPool = threadPool;
        }

        public DelegateThreadPoolQueueToken Enqueue<T>(T userObject, ThreadAction threadAction)
        {
            var queueToken = _threadPool.Enqueue(threadAction);
            queueToken.UserObject = userObject;
            Collection.Add(queueToken);
            return queueToken;
        }

        public DelegateThreadPoolQueueToken Enqueue(ThreadAction threadAction)
        {
            var queueToken = _threadPool.Enqueue(threadAction);
            Collection.Add(queueToken);
            return queueToken;
        }

        public void WaitAll()
        {
            while (Collection.All(o => o.WaitForComplete()) == false)
            {
                Thread.Yield();
            }
        }
    }
}
