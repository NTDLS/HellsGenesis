using HG.Engine;
using HG.Menus;
using HG.TickHandlers.Interfaces;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class MenuTickHandler : IUnvectoredTickManager
    {
        public List<MenuBase> _controller { get; private set; } = new();

        private readonly EngineCore _core;

        public MenuTickHandler(EngineCore core)
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

        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            lock (_controller)
            {
                foreach (var obj in _controller)
                {
                    obj.Render(renderTarget);
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

        public void Insert(MenuBase menu)
        {
            lock (_controller)
            {
                _controller.Add(menu);
            }
        }

        public void Delete(MenuBase menu)
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
