using HG.Actors.Objects.Enemies;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HG.Actors.Factories
{
    internal class EngineActorEnemyManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorEnemyManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HGPoint<double> appliedOffset)
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
                    enemy.ApplyIntelligence(appliedOffset);

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
                        _core.Player.Actor.SelectedSecondaryWeapon.ApplyIntelligence(appliedOffset, enemy); //Player lock-on to enemy. :D
                    }
                }

                enemy.ApplyMotion(appliedOffset);

                if (enemy.ThrustAnimation != null)
                {
                    enemy.ThrustAnimation.Visable = enemy.Velocity.ThrottlePercentage > 0;
                }
            }
        }

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<EnemyBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            return (from o in _manager.OfType<EnemyBase>()
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
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
                obj.Velocity.MaxSpeed = HGRandom.Random.Next(_core.Settings.MinSpeed, _core.Settings.MaxSpeed);
                obj.Velocity.Angle.Degrees = HGRandom.Random.Next(0, 360);

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
    }
}
