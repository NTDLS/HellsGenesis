namespace Si.Engine.Core.Types
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine. We use this so that we can defer 
    /// tasks without sleeping and so we can inject into the sprites during the world clock logic.
    /// </summary>
    public class SiDefermentEvent
    {
        public string? Name { get; set; }
        private readonly object? _parameter = null;
        private readonly int _timeoutMilliseconds;
        private readonly SiDefermentExecuteCallback? _executionCallback = null;
        private readonly SiDefermentSimpleExecuteCallback? _simpleExecutionCallback = null;
        private readonly SiDefermentEventMode _eventMode = SiDefermentEventMode.OneTime;
        private readonly SiDefermentEventThreadModel _threadModel;
        private DateTime _eventTriggerBaseTime;

        public Guid UID { get; private set; }

        public bool IsQueuedForDeletion { get; private set; } = false;

        /// <summary>
        /// Delegate for the event exection callback.
        /// </summary>
        /// <param name="core">Engine core</param>
        /// <param name="sender">The event that is being triggered</param>
        /// <param name="parameter">An optional object passed by the user code</param>
        public delegate void SiDefermentExecuteCallback(SiDefermentEvent sender, object? parameter);

        /// <summary>
        /// Delegate for the event exection callback.
        /// </summary>
        public delegate void SiDefermentSimpleExecuteCallback();

        public enum SiDefermentEventMode
        {
            OneTime,
            Recurring
        }

        public enum SiDefermentEventThreadModel
        {
            Synchronous,
            Asynchronous
        }

        /// <summary>
        /// Creates a new event. This can be a recurring event, single event, synchronous, asynchronous and can be passed parameters.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        /// <param name="eventMode">Whether the event is one time or recurring.</param>
        /// <param name="threadModel">Wheter the event callback is run synchronous or asynchronous.</param>
        public SiDefermentEvent(int timeoutMilliseconds, object parameter, SiDefermentExecuteCallback executionCallback,
            SiDefermentEventMode eventMode = SiDefermentEventMode.OneTime,
            SiDefermentEventThreadModel threadModel = SiDefermentEventThreadModel.Synchronous)
        {
            _parameter = parameter;
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventMode = eventMode;
            _threadModel = threadModel;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new one-time synchronous event that is passed a parameter.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="parameter">An object that will be passed to the execution callback.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        public SiDefermentEvent(int timeoutMilliseconds, object parameter, SiDefermentExecuteCallback executionCallback)
        {
            _parameter = parameter;
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new one-time no-parameter synchronous event.
        /// </summary>
        /// <param name="timeoutMilliseconds">Time until the event is fired.</param>
        /// <param name="executionCallback">The callback function that will be called when the timeout expires.</param>
        public SiDefermentEvent(int timeoutMilliseconds, SiDefermentExecuteCallback executionCallback)
        {
            _timeoutMilliseconds = timeoutMilliseconds;
            _executionCallback = executionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiDefermentEvent(int timeoutMilliseconds, SiDefermentSimpleExecuteCallback simpleExecutionCallback)
        {
            _timeoutMilliseconds = timeoutMilliseconds;
            _simpleExecutionCallback = simpleExecutionCallback;
            _eventTriggerBaseTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public void QueueForDeletion()
        {
            IsQueuedForDeletion = true;
        }

        public bool CheckForTrigger()
        {
            lock (this)
            {
                bool result = false;

                if (IsQueuedForDeletion)
                {
                    return false;
                }

                if ((DateTime.UtcNow - _eventTriggerBaseTime).TotalMilliseconds >= _timeoutMilliseconds)
                {
                    result = true;

                    if (_eventMode == SiDefermentEventMode.OneTime)
                    {
                        IsQueuedForDeletion = true;
                    }

                    if (_threadModel == SiDefermentEventThreadModel.Asynchronous)
                    {
                        new Thread(() =>
                        {
                            if (_executionCallback != null) _executionCallback(this, _parameter);
                            if (_simpleExecutionCallback != null) _simpleExecutionCallback();
                        }).Start();
                    }
                    else
                    {
                        if (_executionCallback != null) _executionCallback(this, _parameter);
                        if (_simpleExecutionCallback != null) _simpleExecutionCallback();
                    }

                    if (_eventMode == SiDefermentEventMode.Recurring)
                    {
                        _eventTriggerBaseTime = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
