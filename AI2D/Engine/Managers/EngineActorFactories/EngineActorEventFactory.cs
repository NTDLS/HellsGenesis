using AI2D.Engine.Menus;
using System;
using System.Collections.Generic;

namespace AI2D.Engine.Managers.EngineActorFactories
{
    internal class EngineActorEventFactory
    {
        public List<EngineCallbackEvent> Collection { get; private set; } = new();

        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorEventFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void QueueTheDoorIsAjar()
        {
            Create(new TimeSpan(0, 0, 0, 5), TheDoorIsAjarCallback);
        }

        private void TheDoorIsAjarCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            _manager.DoorIsAjarSound.Play();
            _manager.MenuFactory.Insert(new MenuStartNewGame(_core));
        }

        public EngineCallbackEvent Create(
            TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj,
            EngineCallbackEvent.CallbackEventMode callbackEventMode = EngineCallbackEvent.CallbackEventMode.OneTime,
            EngineCallbackEvent.CallbackEventAsync callbackEventAsync = EngineCallbackEvent.CallbackEventAsync.Synchronous)
        {
            lock (Collection)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                Collection.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent Create(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj)
        {
            lock (Collection)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj);
                Collection.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent Create(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback)
        {
            lock (Collection)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback);
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
