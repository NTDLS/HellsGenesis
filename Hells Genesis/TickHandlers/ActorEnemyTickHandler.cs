using HG.Actors.Enemies;
using HG.Engine;
using HG.Engine.Controllers;
using HG.TickHandlers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;

namespace HG.TickHandlers
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

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            if (_core.Player.Actor != null)
            {
                _core.Player.Actor.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in _core.Actors.VisibleOfType<EnemyBase>())
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
                obj.Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MinSpeed, _core.Settings.MaxSpeed);
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
