using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Menus;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using System;
using System.Collections.Generic;
using static StrikeforceInfinity.Game.Engine.Types.SiEngineCallbackEvent;

namespace StrikeforceInfinity.Game.Controller
{
    internal class EventTickController : UnvectoredTickControllerBase<SiEngineCallbackEvent>
    {
        public List<SiEngineCallbackEvent> Collection { get; private set; } = new();

        public EventTickController(EngineCore core)
            : base(core)
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
                core.Audio.DoorIsAjarSound.Play();
                core.Menus.Insert(new MenuStartNewGame(core));
            });
        }

        #region Factories.

        public SiEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj,
            HgCallbackEventMode callbackEventMode = HgCallbackEventMode.OneTime,
            HgCallbackEventAsync callbackEventAsync = HgCallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(Core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(Core, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public SiEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback)
        {
            lock (Collection)
            {
                var obj = new SiEngineCallbackEvent(Core, countdown, executeCallback);
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
