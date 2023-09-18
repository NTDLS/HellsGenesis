using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites.Enemies;
using HG.Utility;
using System;
using System.Collections.Generic;

namespace HG.Controller
{
    internal class EnemySpriteTickController : IVectoredTickController
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : SpriteEnemyBase => _controller.VisibleOfType<subType>();
        public List<SpriteEnemyBase> Visible() => _controller.VisibleOfType<SpriteEnemyBase>();
        public List<subType> OfType<subType>() where subType : SpriteEnemyBase => _controller.OfType<subType>();

        public EnemySpriteTickController(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            if (_core.Player.Sprite != null)
            {
                _core.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                enemy.SelectedSecondaryWeapon?.LockedOnObjects.Clear();

                if (_core.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (_core.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        _core.Player.Sprite.SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<SpriteEnemyBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(SpriteEnemyBase obj)
        {
            lock (_controller.Collection)
            {
                _controller.Collection.Add(obj);
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MinEnemySpeed, _core.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Random.Next(0, 360);

                obj.BeforeCreate();
                _controller.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }

        public void Delete(SpriteEnemyBase obj)
        {
            lock (_controller.Collection)
            {
                obj.Cleanup();
                _controller.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
