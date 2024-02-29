using NTDLS.Semaphore;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Menus;
using Si.GameEngine.TickControllers._Superclass;
using System.Collections.Generic;
using static Si.GameEngine.Core.Types.SiEngineCallbackEvent;

namespace Si.GameEngine.TickControllers
{
    public class EventsTickController : UnvectoredTickControllerBase<SiEngineCallbackEvent>
    {
        private readonly PessimisticCriticalResource<List<SiEngineCallbackEvent>> _collection = new();
        public delegate void SiOnExecuteSimpleT<T>(T param);

        public EventsTickController(GameEngineCore gameEngine)
            : base(gameEngine)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    var engineEvent = o[i];
                    if (engineEvent.IsQueuedForDeletion == false)
                    {
                        engineEvent.CheckForTrigger();
                    }
                }
            });
        }

        /// <summary>
        /// We fire this event when the game is won.
        /// </summary>
        public void QueueTheDoorIsAjar()
        {
            Create(4, (sender, refObj) =>
            {
                GameEngine.Audio.DoorIsAjarSound.Play();
                GameEngine.Menus.Add(new MenuStartNewGame(GameEngine));
            });
        }

        #region Factories.

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback, object refObj,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, refObj, executeCallback, callbackEventMode, callbackEventAsync);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, null, executeCallback, callbackEventMode, callbackEventAsync);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, null, executeCallback, callbackEventMode);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, object refObj, SiOnExecute executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, refObj, executeCallback);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, executeCallback);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create<T>(int milliseconds, T param, SiOnExecuteSimpleT<T> executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds,
                    (SiEngineCallbackEvent sender, object refObj) =>
                {
                    executeCallback(param);
                });
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecuteSimple executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(milliseconds, executeCallback);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Add(SiEngineCallbackEvent obj)
        {
            return _collection.Use(o =>
            {
                o.Add(obj);
                return obj;
            });
        }

        public void Delete(SiEngineCallbackEvent obj)
        {
            _collection.Use(o =>
            {
                o.Remove(obj);
            });
        }

        public void CleanupQueuedForDeletion()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    if (o[i].IsQueuedForDeletion)
                    {
                        o.Remove(o[i]);
                    }
                }
            });
        }

        #endregion
    }
}
