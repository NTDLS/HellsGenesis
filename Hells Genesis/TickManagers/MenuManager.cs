using HG.Engine;
using HG.Menus;
using HG.TickManagers.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace HG.TickManagers
{
    internal class MenuManager : IUnvectoredTickManager
    {
        public List<BaseMenu> Collection { get; private set; } = new();

        private readonly Core _core;

        public MenuManager(Core core)
        {
            _core = core;
        }

        public void ExecuteWorldClockTick()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                var menu = Collection[i];
                menu.HandleInput();
            }
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

        #region Factories.

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

        #endregion
    }
}
