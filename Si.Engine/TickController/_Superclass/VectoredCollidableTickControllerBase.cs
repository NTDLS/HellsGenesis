using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass._SpriteBase;
using Si.Library.Mathematics;
using System;
using System.Linq;

namespace Si.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class VectoredCollidableTickControllerBase<T> : ITickController<T> where T : SpriteBase
    {
        public EngineCore Engine { get; private set; }
        public SpriteManager SpriteManager { get; private set; }

        public subType[] VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public T[] Visible() => SpriteManager.VisibleOfType<T>();
        public T[] All() => SpriteManager.OfType<T>();
        public subType[] OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T ByTag(string name) => SpriteManager.VisibleOfType<T>().Where(o => o.SpriteTag == name).FirstOrDefault();

        public virtual void ExecuteWorldClockTick(float epoch, SiVector displacementVector) { }

        public VectoredCollidableTickControllerBase(EngineCore engine, SpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        public void QueueAllForDeletion() => SpriteManager.QueueAllForDeletionOfType<T>();

        public void Add(T obj) => SpriteManager.Add(obj);

        public T Add(string bitmapPath, SiVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(string bitmapPath, float x, float y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
            obj.X = x;
            obj.Y = y;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterScreen()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = Engine.Display.CenterOfCurrentScreen;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterScreen(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
            obj.Location = Engine.Display.CenterOfCurrentScreen;
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterUniverse()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.X = Engine.Display.TotalCanvasSize.Width / 2;
            obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAtCenterUniverse(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
            obj.X = Engine.Display.TotalCanvasSize.Width / 2;
            obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(float x, float y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.X = x;
            obj.Y = y;
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(SiVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(SpriteBase locationOf)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = locationOf.Location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(SiVector location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T AddAt(SharpDX.Direct2D1.Bitmap bitmap, SpriteBase locationOf)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap);
            obj.Location = locationOf.Location.Clone();
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(string bitmapPath)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
            SpriteManager.Add(obj);
            return obj;
        }

        public T Add(SharpDX.Direct2D1.Bitmap bitmap)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap);
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T), Engine);
        }

        public T Create(string bitmapPath)
        {
            return (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath);
        }
    }
}
