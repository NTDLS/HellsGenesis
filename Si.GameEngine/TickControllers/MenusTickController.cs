using NTDLS.Semaphore;
using SharpDX.Direct2D1;
using Si.GameEngine.Engine;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Controller
{
    public class MenusTickController : UnvectoredTickControllerBase<MenuBase>
    {
        public delegate void CollectionAccessor(List<MenuBase> sprites);
        public delegate T CollectionAccessorT<T>(List<MenuBase> sprites);

        private readonly PessimisticSemaphore<List<MenuBase>> _collection = new();

        /// <summary>
        /// Determines if any active menu handles the escape key.
        /// </summary>
        /// <returns></returns>
        public bool VisibleMenuHandlesEscape()
        {
            return _collection.Use(o =>
                o.Where(o => o.HandlesEscape() == true).Any());
        }

        public MenusTickController(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public void Use(CollectionAccessor collectionAccessor)
            => _collection.Use(o => collectionAccessor(o));

        public T Use<T>(CollectionAccessorT<T> collectionAccessor)
            => _collection.Use<T>(o => collectionAccessor(o));

        public override void ExecuteWorldClockTick()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    var menu = o[i];
                    menu.HandleInput();
                }
            });
        }

        public void Render(RenderTarget renderTarget)
        {
            _collection.Use(o =>
                o.ForEach(obj => obj.Render(renderTarget)));
        }

        #region Factories.

        public void CleanupDeletedObjects()
        {
            _collection.Use(o =>
            {
                for (int i = 0; i < o.Count; i++)
                {
                    if (o[i].QueuedForDeletion)
                    {
                        Delete(o[i]);
                    }
                }
            });
        }

        public void Add(MenuBase menu)
            => _collection.Use(o => o.Add(menu));

        public void Delete(MenuBase menu)
        {
            _collection.Use(o =>
            {
                menu.InvokeCleanup();
                o.Remove(menu);
            });
        }

        #endregion
    }
}
