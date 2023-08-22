using AI2D.Engine.Menus;
using System;
using System.Collections.Generic;
using static AI2D.Engine.EngineCallbackEvent;

namespace AI2D.Engine.Managers
{
    internal class EngineEventManager
    {
        public List<EngineCallbackEvent> Collection { get; private set; } = new();

        private readonly Core _core;

        public EngineEventManager(Core core)
        {
            _core = core;
        }

        public void Execute()
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
                core.Actors.Menus.Insert(new MenuStartNewGame(core));
            });
        }

        public EngineCallbackEvent Create(TimeSpan countdown, OnExecute executeCallback, object refObj,
            CallbackEventMode callbackEventMode = CallbackEventMode.OneTime,
            CallbackEventAsync callbackEventAsync = CallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                var obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent Create(TimeSpan countdown, OnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                var obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent Create(TimeSpan countdown, OnExecute executeCallback)
        {
            lock (Collection)
            {
                var obj = new EngineCallbackEvent(_core, countdown, executeCallback);
                Collection.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent Insert(EngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(EngineCallbackEvent obj)
        {
            lock (Collection)
            {
                Collection.Remove(obj);
            }
        }
    }
}
