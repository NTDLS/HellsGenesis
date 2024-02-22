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
        private readonly GameEngineCore _gameEngine;
        private readonly object _referenceObject = null;
        private readonly TimeSpan _countdown;
        private readonly SiOnExecute _onExecute;
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
        public delegate void SiOnExecute(GameEngineCore gameEngine, SiEngineCallbackEvent sender, object refObj);

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

        public SiEngineCallbackEvent(GameEngineCore gameEngine, TimeSpan countdown, SiOnExecute executeCallback, object refObj,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            _gameEngine = gameEngine;
            _referenceObject = refObj;
            _countdown = countdown;
            _onExecute = executeCallback;
            _callbackEventMode = callbackEventMode;
            _callbackEventAsync = callbackEventAsync;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiEngineCallbackEvent(GameEngineCore gameEngine, TimeSpan countdown, SiOnExecute executeCallback, object refObj)
        {
            _referenceObject = refObj;
            _gameEngine = gameEngine;
            _countdown = countdown;
            _onExecute = executeCallback;
            _startedTime = DateTime.UtcNow;
            UID = Guid.NewGuid();
        }

        public SiEngineCallbackEvent(GameEngineCore gameEngine, TimeSpan countdown, SiOnExecute executeCallback)
        {
            _gameEngine = gameEngine;
            _countdown = countdown;
            _onExecute = executeCallback;
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

                if ((DateTime.UtcNow - _startedTime).TotalMilliseconds > _countdown.TotalMilliseconds)
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
                            _onExecute(_gameEngine, this, _referenceObject);
                        }).Start();
                    }
                    else
                    {
                        _onExecute(_gameEngine, this, _referenceObject);
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
