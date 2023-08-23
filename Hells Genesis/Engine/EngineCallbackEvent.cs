using System;
using System.Threading;

namespace HG.Engine
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine. We use this so that we can
    /// defer things without sleeping and so we can inject into the actors durring the frame logic.
    /// </summary>
    internal class EngineCallbackEvent
    {
        private readonly Core _core;
        private readonly object _referenceObject = null;
        private readonly TimeSpan _countdown;
        private readonly OnExecute _onExecute;
        private readonly CallbackEventMode _callbackEventMode;
        private readonly CallbackEventAsync _callbackEventAsync;
        private DateTime _startedTime;

        public Guid UID { get; private set; }

        public bool ReadyForDeletion { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="core">Engine core</param>
        /// <param name="sender">The event that is being triggered</param>
        /// <param name="refObj">An optional object passed by the user code</param>
        public delegate void OnExecute(Core core, EngineCallbackEvent sender, object refObj);

        public enum CallbackEventMode
        {
            OneTime,
            Recurring
        }

        public enum CallbackEventAsync
        {
            Synchronous,
            Asynchronous
        }

        public EngineCallbackEvent(Core core, TimeSpan countdown, OnExecute executeCallback, object refObj,
            CallbackEventMode callbackEventMode = CallbackEventMode.OneTime,
            CallbackEventAsync callbackEventAsync = CallbackEventAsync.Synchronous)
        {
            _core = core;
            _referenceObject = refObj;
            _countdown = countdown;
            _onExecute = executeCallback;
            _callbackEventMode = callbackEventMode;
            _callbackEventAsync = callbackEventAsync;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public EngineCallbackEvent(Core core, TimeSpan countdown, OnExecute executeCallback, object refObj)
        {
            _core = core;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public EngineCallbackEvent(Core core, TimeSpan countdown, OnExecute executeCallback)
        {
            _core = core;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public bool CheckForTrigger()
        {
            lock (this)
            {
                bool result = false;

                if (ReadyForDeletion)
                {
                    return false;
                }

                if ((DateTime.UtcNow - _startedTime).TotalMilliseconds > _countdown.TotalMilliseconds)
                {
                    result = true;

                    if (_callbackEventMode == CallbackEventMode.OneTime)
                    {
                        ReadyForDeletion = true;
                    }

                    if (_callbackEventAsync == CallbackEventAsync.Asynchronous)
                    {
                        new Thread(() =>
                        {
                            _onExecute(_core, this, _referenceObject);
                        }).Start();
                    }
                    else
                    {
                        _onExecute(_core, this, _referenceObject);
                    }

                    if (_callbackEventMode == CallbackEventMode.Recurring)
                    {
                        _startedTime = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
