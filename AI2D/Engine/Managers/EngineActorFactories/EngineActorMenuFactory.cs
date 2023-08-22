using AI2D.Engine.Menus;
using System.Collections.Generic;
using System.Drawing;

namespace AI2D.Engine.Managers.EngineActorFactories
{
    public class EngineActorMenuFactory
    {
        public List<BaseMenu> Collection { get; private set; } = new();

        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorMenuFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void Render(Graphics dc)
        {
            lock (Collection)
            {
                foreach (var obj in Collection)
                {
                    obj.Render(dc);
                }
            }
        }

        public void Insert(BaseMenu menu)
        {
            lock (Collection)
            {
                Collection.Add(menu);
            }
        }

        public void Delete(BaseMenu menu)
        {
            lock (Collection)
            {
                menu.Cleanup();
                Collection.Remove(menu);
            }
        }
    }
}
