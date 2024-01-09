using NTDLS.Semaphore;
using Si.GameEngine.Engine;
using Si.GameEngine.Engine.Types;
using Si.GameEngine.Menus;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using System;
using System.Collections.Generic;
using static Si.GameEngine.Engine.Types.SiEngineCallbackEvent;

namespace Si.GameEngine.Controller
{
    public class EventsTickController : UnvectoredTickControllerBase<SiEngineCallbackEvent>
    {
        private readonly PessimisticSemaphore<List<SiEngineCallbackEvent>> _collection = new();

        public EventsTickController(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    var engineEvent = o[i];
                    if (engineEvent.QueuedForDeletion == false)
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
            Create(new TimeSpan(0, 0, 0, 5), (core, sender, refObj) =>
            {
                GameCore.Audio.DoorIsAjarSound.Play();
                GameCore.Menus.Add(new MenuStartNewGame(core));
            });
        }

        #region Factories.

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback, object refObj,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback, object refObj)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback, refObj);
                o.Add(obj);
                return obj;
            });
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback)
        {
            return _collection.Use(o =>
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback);
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
                    if (o[i].QueuedForDeletion)
                    {
                        o.Remove(o[i]);
                    }
                }
            });
        }

        #endregion
    }
}
