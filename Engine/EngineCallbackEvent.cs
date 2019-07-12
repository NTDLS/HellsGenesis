using System;
using System.Threading;

namespace AI2D.Engine
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine.
    /// We use this so that we can defer things without sleeping and so we can inject into the actors durring the frame logic.
    /// </summary>
    public class EngineCallbackEvent
    {
        private Core _core;
        private object _referenceObject = null;
        private TimeSpan _countdown;
        private OnExecute _onExecute;
        private CallbackEventMode _callbackEventMode;
        private CallbackEventAsync _callbackEventAsync;
        private DateTime _startedTime;

        public bool ReadyForDeletion = false;

        public delegate void OnExecute(Core core, object refObj);

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
        }

        public EngineCallbackEvent(Core core, TimeSpan countdown, OnExecute executeCallback, object refObj)
        {
            _core = core;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
        }

        public EngineCallbackEvent(Core core, TimeSpan countdown, OnExecute executeCallback)
        {
            _core = core;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
        }

        public bool CheckForTrigger()
        {
            bool result = false;

            if (ReadyForDeletion)
            {
                return false;
            }

            if ((DateTime.UtcNow - _startedTime).TotalMilliseconds > _countdown.TotalMilliseconds)
            {
                result = true;

                if (_callbackEventAsync == CallbackEventAsync.Asynchronous)
                {
                    new Thread(() =>
                    {
                        _onExecute(_core, _referenceObject);
                    }).Start();
                }
                else
                {
                    _onExecute(_core, _referenceObject);
                }

                if (_callbackEventMode == CallbackEventMode.Recurring)
                {
                    _startedTime = DateTime.UtcNow;
                }
                else
                {
                    ReadyForDeletion = true;
                }
            }

            return result;
        }
    }
}
