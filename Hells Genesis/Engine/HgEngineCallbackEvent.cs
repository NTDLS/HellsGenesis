using System;
using System.Threading;

namespace HG.Engine
{
    /// <summary>
    /// Allows for deferred events to be injected into the engine. We use this so that we can
    /// defer things without sleeping and so we can inject into the actors durring the frame logic.
    /// </summary>
    internal class HgEngineCallbackEvent
    {
        private readonly Core _core;
        private readonly object _referenceObject = null;
        private readonly TimeSpan _countdown;
        private readonly HgOnExecute _onExecute;
        private readonly HgCallbackEventMode _callbackEventMode;
        private readonly HgCallbackEventAsync _callbackEventAsync;
        private DateTime _startedTime;

        public Guid UID { get; private set; }

        public bool ReadyForDeletion { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="core">Engine core</param>
        /// <param name="sender">The event that is being triggered</param>
        /// <param name="refObj">An optional object passed by the user code</param>
        public delegate void HgOnExecute(Core core, HgEngineCallbackEvent sender, object refObj);

        public enum HgCallbackEventMode
        {
            OneTime,
            Recurring
        }

        public enum HgCallbackEventAsync
        {
            Synchronous,
            Asynchronous
        }

        public HgEngineCallbackEvent(Core core, TimeSpan countdown, HgOnExecute executeCallback, object refObj,
            HgCallbackEventMode callbackEventMode = HgCallbackEventMode.OneTime,
            HgCallbackEventAsync callbackEventAsync = HgCallbackEventAsync.Synchronous)
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

        public HgEngineCallbackEvent(Core core, TimeSpan countdown, HgOnExecute executeCallback, object refObj)
        {
            _core = core;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public HgEngineCallbackEvent(Core core, TimeSpan countdown, HgOnExecute executeCallback)
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

                    if (_callbackEventMode == HgCallbackEventMode.OneTime)
                    {
                        ReadyForDeletion = true;
                    }

                    if (_callbackEventAsync == HgCallbackEventAsync.Asynchronous)
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

                    if (_callbackEventMode == HgCallbackEventMode.Recurring)
                    {
                        _startedTime = DateTime.UtcNow;
                    }
                }
                return result;
            }
        }
    }
}
