using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class SpriteTickControllerBase<T> : TickControllerBase<T> where T : SpriteBase
    {
        public EngineCore Engine { get; private set; }
        public EngineSpriteManager SpriteManager { get; private set; }

        public List<subType> VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public List<T> Visible() => SpriteManager.VisibleOfType<T>();
        public List<T> All() => SpriteManager.OfType<T>();
        public List<subType> OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T ByTag(string name) => SpriteManager.VisibleOfType<T>().Where(o => o.SpriteTag == name).FirstOrDefault();

        public virtual void ExecuteWorldClockTick(float epoch, SiPoint displacementVector) { }

        public SpriteTickControllerBase(EngineCore engine, EngineSpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        public void QueueAllForDeletion() => SpriteManager.QueueAllForDeletionOfType<T>();

        public void Add(T obj) => SpriteManager.Add(obj);

        public T Create(SiPoint location, string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = location.Clone();
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(float x, float y, string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.X = x;
            obj.Y = y;
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public T CreateAtCenterScreen(string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.X = Engine.Display.TotalCanvasSize.Width / 2;
            obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

            obj.SpriteTag = name;

            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(float x, float y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.X = x;
            obj.Y = y;
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(SiPoint location)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.Location = location;
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine);
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
