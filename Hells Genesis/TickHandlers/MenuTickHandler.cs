using HG.Engine;
using HG.Menus;
using HG.TickHandlers.Interfaces;
using System.Collections.Generic;
using System.Drawing;

namespace HG.TickHandlers
{
    internal class MenuTickHandler : IUnvectoredTickManager
    {
        public List<BaseMenu> _controller { get; private set; } = new();

        private readonly Core _core;

        public MenuTickHandler(Core core)
        {
            _core = core;
        }

        public void ExecuteWorldClockTick()
        {
            for (int i = 0; i < _controller.Count; i++)
            {
                var menu = _controller[i];
                menu.HandleInput();
            }
        }

        public void Render(Graphics dc)
        {
            lock (_controller)
            {
                foreach (var obj in _controller)
                {
                    obj.Render(dc);
                }
            }
        }

        #region Factories.

        public void CleanupDeletedObjects()
        {
            for (int i = 0; i < _controller.Count; i++)
            {
                if (_controller[i].ReadyForDeletion)
                {
                    Delete(_controller[i]);
                }
            }
        }

        public void Insert(BaseMenu menu)
        {
            lock (_controller)
            {
                _controller.Add(menu);
            }
        }

        public void Delete(BaseMenu menu)
        {
            lock (_controller)
            {
                menu.Cleanup();
                _controller.Remove(menu);
            }
        }

        #endregion
    }
}
