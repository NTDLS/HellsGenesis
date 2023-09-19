using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.TickControllers
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// </summary>
    internal class VectoredTickControllerBase<T> : TickControllerBase<T> where T : SpriteBase
    {
        public EngineCore Core { get; private set; }
        public EngineSpriteManager SpriteManager { get; private set; }

        public List<subType> VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public List<T> Visible() => SpriteManager.VisibleOfType<T>();
        public List<T> All() => SpriteManager.OfType<T>();
        public List<subType> OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T ByAssetTag(string name) => SpriteManager.VisibleOfType<T>().Where(o => o.Name == name).FirstOrDefault();

        public virtual void ExecuteWorldClockTick(HgPoint displacementVector) { }

        public VectoredTickControllerBase(EngineCore core, EngineSpriteManager manager)
        {
            Core = core;
            SpriteManager = manager;
        }

        public void DeleteAll()
        {
            lock (SpriteManager.Collection)
            {
                SpriteManager.OfType<T>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(T obj)
        {
            lock (SpriteManager.Collection)
            {
                SpriteManager.Collection.Add(obj);
            }
        }

        public T Create(double x, double y, string name = "")
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), Core);
                obj.X = x;
                obj.Y = y;
                obj.Name = name; SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T CreateAtCenterScreen(string name = "")
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), Core);
                obj.X = Core.Display.TotalCanvasSize.Width / 2;
                obj.Y = Core.Display.TotalCanvasSize.Height / 2;

                obj.Name = name;

                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T Create(double x, double y)
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), Core);
                obj.X = x;
                obj.Y = y;
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T Create()
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), Core);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T Create(string name = "")
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), Core);
                obj.Name = name;
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(T obj)
        {
            lock (SpriteManager.Collection)
            {
                obj.Cleanup();
                SpriteManager.Collection.Remove(obj);
            }
        }
    }
}
