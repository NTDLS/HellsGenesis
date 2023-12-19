using SharpDX.Direct2D1;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace StrikeforceInfinity.Game.Controller
{
    internal class MenuTickController : UnvectoredTickControllerBase<MenuBase>
    {
        public List<MenuBase> Collection { get; private set; } = new();

        /// <summary>
        /// Determines if any active menu handles the escape key.
        /// </summary>
        /// <returns></returns>
        public bool VisibleMenuHandlesEscape()
        {
            lock (Collection)
            {
                return Collection.Where(o => o.HandlesEscape() == true).Any();
            }
        }

        public MenuTickController(EngineCore core)
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

        public void Render(RenderTarget renderTarget)
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
                if (Collection[i].QueuedForDeletion)
                {
                    Delete(Collection[i]);
                }
            }
        }

        public void Insert(MenuBase menu)
        {
            lock (Collection)
            {
                Collection.Add(menu);
            }
        }

        public void Delete(MenuBase menu)
        {
            lock (Collection)
            {
                menu.InvokeCleanup();
                Collection.Remove(menu);
            }
        }

        #endregion
    }
}
