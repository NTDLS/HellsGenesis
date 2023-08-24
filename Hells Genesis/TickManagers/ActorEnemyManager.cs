using HG.Actors.Enemies;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class ActorEnemyManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : EnemyBase => _manager.VisibleOfType<subType>();
        public List<EnemyBase> VisibleOfType() => _manager.VisibleOfType<EnemyBase>();
        public List<subType> OfType<subType>() where subType : EnemyBase => _manager.OfType<subType>();

        public ActorEnemyManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
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

                    //Player collides with enemy.
                    if (enemy.Intersects(_core.Player.Actor))
                    {
                        if (_core.Player.Actor.Hit(enemy.CollisionDamage, true, false))
                        {
                            _core.Player.Actor.HitExplosion();
                            //enemy.Hit(enemy.CollisionDamage);
                        }
                    }

                    if (_core.Player.Actor.SelectedSecondaryWeapon != null)
                    {
                        _core.Player.Actor.SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
                }

                enemy.ApplyMotion(displacementVector);

                if (enemy.ThrustAnimation != null)
                {
                    enemy.ThrustAnimation.Visable = enemy.Velocity.ThrottlePercentage > 0;
                }
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<EnemyBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void Insert(EnemyBase obj)
        {
            lock (_manager.Collection)
            {
                _manager.Collection.Add(obj);
            }
        }

        public T Create<T>() where T : EnemyBase
        {
            lock (_manager.Collection)
            {
                object[] param = { _core };
                EnemyBase obj = (EnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MinSpeed, _core.Settings.MaxSpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Random.Next(0, 360);

                obj.BeforeCreate();
                _manager.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }

        public void Delete(EnemyBase obj)
        {
            lock (_manager.Collection)
            {
                obj.Cleanup();
                _manager.Collection.Remove(obj);
            }
        }

        #endregion
    }
}
