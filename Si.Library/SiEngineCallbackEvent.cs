using System;
using System.Threading;

namespace Si.GameEngine.Core.Types
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine. We use this so that we can
    /// defer things without sleeping and so we can inject into the sprites durring the frame logic.
    /// </summary>
    public class SiEngineCallbackEvent
    {
        public string? Name { get; set; }
        private readonly object? _referenceObject = null;
        private readonly int _milliseconds;
        private readonly SiOnExecute? _onExecute = null;
        private readonly SiOnExecuteSimple? _onExecuteSimple = null;
        private readonly SiCallbackEventMode _callbackEventMode = SiCallbackEventMode.OneTime;
        private readonly SiCallbackEventAsync _callbackEventAsync;
        private DateTime _startedTime;

        public Guid UID { get; private set; }

        public bool IsQueuedForDeletion { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="core">Engine core</param>
        /// <param name="sender">The event that is being triggered</param>
        /// <param name="refObj">An optional object passed by the user code</param>
        public delegate void SiOnExecute(SiEngineCallbackEvent sender, object? refObj);

        public delegate void SiOnExecuteSimple();

        public enum SiCallbackEventMode
        {
            OneTime,
            Recurring
        }

        public enum SiCallbackEventAsync
        {
            Synchronous,
            Asynchronous
        }

        public SiEngineCallbackEvent(int milliseconds, object refObj, SiOnExecute executeCallback,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            _referenceObject = refObj;
            _milliseconds = milliseconds;
            _onExecute = executeCallback;
            _callbackEventMode = callbackEventMode;
            _callbackEventAsync = callbackEventAsync;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiEngineCallbackEvent(int milliseconds, object refObj, SiOnExecute executeCallback)
        {
            _referenceObject = refObj;
            _milliseconds = milliseconds;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiEngineCallbackEvent(int milliseconds, SiOnExecute executeCallback)
        {
            _milliseconds = milliseconds;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiEngineCallbackEvent(int milliseconds, SiOnExecuteSimple executeCallback)
        {
            _milliseconds = milliseconds;
            _onExecuteSimple = executeCallback;
            _startedTime = DateTime.UtcNow;
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

                if ((DateTime.UtcNow - _startedTime).TotalMilliseconds > _milliseconds)
                {
                    result = true;

                    if (_callbackEventMode == SiCallbackEventMode.OneTime)
                    {
                        IsQueuedForDeletion = true;
                    }

                    if (_callbackEventAsync == SiCallbackEventAsync.Asynchronous)
                    {
                        new Thread(() =>
                        {
                            if (_onExecute != null) _onExecute(this, _referenceObject);
                            if (_onExecuteSimple != null) _onExecuteSimple();
                        }).Start();
                    }
                    else
                    {
                        if (_onExecute != null) _onExecute(this, _referenceObject);
                        if (_onExecuteSimple != null) _onExecuteSimple();
                    }

                    if (_callbackEventMode == SiCallbackEventMode.Recurring)
                    {
                        _startedTime = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
