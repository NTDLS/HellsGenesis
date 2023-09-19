using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Menus;
using NebulaSiege.TickControllers;
using System;
using System.Collections.Generic;
using static NebulaSiege.Engine.Types.NsEngineCallbackEvent;

namespace NebulaSiege.Controller
{
    internal class EventTickController : _UnvectoredTickControllerBase<NsEngineCallbackEvent>
    {
        public List<NsEngineCallbackEvent> Collection { get; private set; } = new();

        public EventTickController(EngineCore core)
            : base(core)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                var engineEvent = Collection[i];
                if (engineEvent.ReadyForDeletion == false)
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

        public NsEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj,
            HgCallbackEventMode callbackEventMode = HgCallbackEventMode.OneTime,
            HgCallbackEventAsync callbackEventAsync = HgCallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                var obj = new NsEngineCallbackEvent(Core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public NsEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                var obj = new NsEngineCallbackEvent(Core, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public NsEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback)
        {
            lock (Collection)
            {
                var obj = new NsEngineCallbackEvent(Core, countdown, executeCallback);
                Collection.Add(obj);
                return obj;
            }
        }

        public NsEngineCallbackEvent Insert(NsEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(NsEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Remove(obj);
            }
        }

        #endregion
    }
}
