using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.Shared.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.TickControllers.BasesAndInterfaces
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class SpriteTickControllerBase<T> : TickControllerBase<T> where T : SpriteBase
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

        public void DeleteAll() => SpriteManager.DeleteAllOfType<T>();

        public void Insert(T obj) => SpriteManager.Add(obj);

        public T Create(double x, double y, string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
            obj.LocalX = x;
            obj.LocalY = y;
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public T CreateAtCenterScreen(string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
            obj.LocalX = GameCore.Display.TotalCanvasSize.Width / 2;
            obj.LocalY = GameCore.Display.TotalCanvasSize.Height / 2;

            obj.SpriteTag = name;

            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(double x, double y)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
            obj.LocalX = x;
            obj.LocalY = y;
            SpriteManager.Add(obj);
            return obj;

        }

        public T Create()
        {
            T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
            SpriteManager.Add(obj);
            return obj;
        }

        public T Create(string name = "")
        {
            T obj = (T)Activator.CreateInstance(typeof(T), GameCore);
            obj.SpriteTag = name;
            SpriteManager.Add(obj);
            return obj;
        }

        public void Delete(T obj)
            => SpriteManager.Delete(obj);
    }
}
