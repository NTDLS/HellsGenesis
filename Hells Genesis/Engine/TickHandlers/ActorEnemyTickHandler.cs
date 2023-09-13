using HG.Actors.Enemies.BaseClasses;
using HG.Engine.Controllers;
using HG.Engine.TickHandlers.Interfaces;
using HG.Types.Geometry;
using HG.Utility;
using System;
using System.Collections.Generic;

namespace HG.Engine.TickHandlers
{
    internal class ActorEnemyTickHandler : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorController _controller;

        public List<subType> VisibleOfType<subType>() where subType : EnemyBase => _controller.VisibleOfType<subType>();
        public List<EnemyBase> Visible() => _controller.VisibleOfType<EnemyBase>();
        public List<subType> OfType<subType>() where subType : EnemyBase => _controller.OfType<subType>();

        public ActorEnemyTickHandler(Core core, EngineActorController manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            if (_core.Player.Actor != null)
            {
                _core.Player.Actor.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                enemy.SelectedSecondaryWeapon?.LockedOnObjects.Clear();

                if (_core.Player.Actor.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (_core.Player.Actor.SelectedSecondaryWeapon != null)
                    {
                        _core.Player.Actor.SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, enemy); //Player lock-on to enemy. :D
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
                _controller.OfType<EnemyBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(EnemyBase obj)
        {
            lock (_controller.Collection)
            {
                _controller.Collection.Add(obj);
            }
        }

        public T Create<T>() where T : EnemyBase
        {
            lock (_controller.Collection)
            {
                object[] param = { _core };
                EnemyBase obj = (EnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Random.Next(Settings.MinSpeed, Settings.MaxSpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Random.Next(0, 360);

                obj.BeforeCreate();
                _controller.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }

        public void Delete(EnemyBase obj)
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
