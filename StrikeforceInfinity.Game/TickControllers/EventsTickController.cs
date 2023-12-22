using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Menus;
using StrikeforceInfinity.Game.TickControllers.BasesAndInterfaces;
using System;
using System.Collections.Generic;
using static StrikeforceInfinity.Game.Engine.Types.SiEngineCallbackEvent;

namespace StrikeforceInfinity.Game.Controller
{
    internal class EventsTickController : UnvectoredTickControllerBase<SiEngineCallbackEvent>
    {
        public List<SiEngineCallbackEvent> Collection { get; private set; } = new();

        public EventsTickController(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                var engineEvent = Collection[i];
                if (engineEvent.QueuedForDeletion == false)
                {
                    engineEvent.CheckForTrigger();
                }
            }
        }

        /// <summary>
        /// We fire this event when the game is won.
        /// </summary>
        public void QueueTheDoorIsAjar()
        {
            Create(new TimeSpan(0, 0, 0, 5), (core, sender, refObj) =>
            {
                GameCore.Audio.DoorIsAjarSound.Play();
                GameCore.Menus.Insert(new MenuStartNewGame(core));
            });
        }

        #region Factories.

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback, object refObj,
            SiCallbackEventMode callbackEventMode = SiCallbackEventMode.OneTime,
            SiCallbackEventAsync callbackEventAsync = SiCallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, SiOnExecute executeCallback)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(GameCore, countdown, executeCallback);
                Collection.Add(obj);
                return obj;
            }
        }

        public SiEngineCallbackEvent Insert(SiEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(SiEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Remove(obj);
            }
        }

        #endregion
    }
}
