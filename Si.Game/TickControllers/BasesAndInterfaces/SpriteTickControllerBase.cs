using Si.Game.Engine;
using Si.Game.Managers;
using Si.Game.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using Si.Shared.Types.Geometry;

namespace Si.Game.TickControllers.BasesAndInterfaces
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    internal class SpriteTickControllerBase<T> : TickControllerBase<T> where T : SpriteBase
    {
        public EngineCore GameCore { get; private set; }
        public EngineSpriteManager SpriteManager { get; private set; }

        public List<subType> VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public List<T> Visible() => SpriteManager.VisibleOfType<T>();
        public List<T> All() => SpriteManager.OfType<T>();
        public List<subType> OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T ByTag(string name) => SpriteManager.VisibleOfType<T>().Where(o => o.SpriteTag == name).FirstOrDefault();

        public virtual void ExecuteWorldClockTick(SiPoint displacementVector) { }

        public SpriteTickControllerBase(EngineCore gameCore, EngineSpriteManager manager)
        {
            GameCore = gameCore;
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
                T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
                obj.X = x;
                obj.Y = y;
                obj.SpriteTag = name; SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T CreateAtCenterScreen(string name = "")
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
                obj.X = GameCore.Display.TotalCanvasSize.Width / 2;
                obj.Y = GameCore.Display.TotalCanvasSize.Height / 2;

                obj.SpriteTag = name;

                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T Create(double x, double y)
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
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
                T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public T Create(string name = "")
        {
            lock (SpriteManager.Collection)
            {
                T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
                obj.SpriteTag = name;
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
