using NTDLS.Semaphore;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Menus;
using System;
using System.Collections.Generic;
using static Si.GameEngine.Core.Types.SiEngineCallbackEvent;

namespace Si.GameEngine.Core.TickControllers
{
    public class EventsTickController : UnvectoredTickControllerBase<SiEngineCallbackEvent>
    {
        private readonly PessimisticCriticalResource<List<SiEngineCallbackEvent>> _collection = new();

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
            Create(4, (core, sender, refObj) =>
            {
                GameEngine.Audio.DoorIsAjarSound.Play();
                GameEngine.Menus.Add(new MenuStartNewGame(core));
            });
        }

        #region Factories.

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback, object refObj,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, refObj, executeCallback, callbackEventMode, callbackEventAsync);
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
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, executeCallback, null, callbackEventMode, callbackEventAsync);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, executeCallback, null, callbackEventMode);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, object refObj, SiOnExecute executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, refObj, executeCallback);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecute executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, executeCallback);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(int milliseconds, SiOnExecuteSimple executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameEngine, milliseconds, executeCallback);
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
