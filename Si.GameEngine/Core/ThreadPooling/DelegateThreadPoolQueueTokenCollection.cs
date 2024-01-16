using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Si.GameEngine.Core.ThreadPooling
{
    public class DelegateThreadPoolQueueTokenCollection
    {
        public List<DelegateThreadPoolQueueToken> Collection { get; private set; } = new();

        public void Add(DelegateThreadPoolQueueToken queueToken)
        {
            Collection.Add(queueToken);
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
