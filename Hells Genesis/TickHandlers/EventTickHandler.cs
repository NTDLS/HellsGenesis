using HG.Engine;
using HG.Engine.Types;
using HG.Menus;
using HG.TickHandlers.Interfaces;
using System;
using System.Collections.Generic;
using static HG.Engine.Types.HgEngineCallbackEvent;

namespace HG.TickHandlers
{
    internal class EventTickHandler : IUnvectoredTickManager
    {
        public List<HgEngineCallbackEvent> Collection { get; private set; } = new();

        private readonly EngineCore _core;

        public EventTickHandler(EngineCore core)
        {
            _core = core;
        }

        public void ExecuteWorldClockTick()
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

        public void QueueTheDoorIsAjar()
        {
            Create(new TimeSpan(0, 0, 0, 5), (core, sender, refObj) =>
            {
                core.Audio.DoorIsAjarSound.Play();
                core.Menus.Insert(new MenuStartNewGame(core));
            });
        }

        #region Factories.

        public HgEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj,
            HgCallbackEventMode callbackEventMode = HgCallbackEventMode.OneTime,
            HgCallbackEventAsync callbackEventAsync = HgCallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                var obj = new HgEngineCallbackEvent(_core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public HgEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                var obj = new HgEngineCallbackEvent(_core, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public HgEngineCallbackEvent Create(TimeSpan countdown, HgOnExecute executeCallback)
        {
            lock (Collection)
            {
                var obj = new HgEngineCallbackEvent(_core, countdown, executeCallback);
                Collection.Add(obj);
                return obj;
            }
        }

        public HgEngineCallbackEvent Insert(HgEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(HgEngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Remove(obj);
            }
        }

        #endregion
    }
}
