using HG.Engine.Menus;
using System.Collections.Generic;
using System.Drawing;

namespace HG.Engine.Managers
{
    internal class EngineMenuManager
    {
        public List<BaseMenu> Collection { get; private set; } = new();

        private readonly Core _core;

        public EngineMenuManager(Core core)
        {
            _core = core;
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

        public void HandleInput()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                var menu = Collection[i];
                menu.HandleInput();
            }
        }

        public void CleanupDeletedObjects()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                if (Collection[i].ReadyForDeletion)
                {
                    Delete(Collection[i]);
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
