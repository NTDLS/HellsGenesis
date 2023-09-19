using HG.Engine;
using HG.Menus;
using HG.TickControllers;
using System.Collections.Generic;

namespace HG.Controller
{
    internal class MenuTickHandler : _UnvectoredTickControllerBase<_MenuBase>
    {
        public List<_MenuBase> Collection { get; private set; } = new();

        public MenuTickHandler(EngineCore core)
            : base(core)
        {
        }

        public override void ExecuteWorldClockTick()
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                var menu = Collection[i];
                menu.HandleInput();
            }
        }

        public void Render(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            lock (Collection)
            {
                foreach (var obj in Collection)
                {
                    obj.Render(renderTarget);
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

        public void Insert(_MenuBase menu)
        {
            lock (Collection)
            {
                Collection.Add(menu);
            }
        }

        public void Delete(_MenuBase menu)
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
