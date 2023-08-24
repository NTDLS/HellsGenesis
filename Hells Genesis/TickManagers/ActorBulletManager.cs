using HG.Actors;
using HG.Actors.Enemies;
using HG.Actors.Weapons;
using HG.Actors.Weapons.Bullets;
using HG.Engine;
using HG.Engine.Managers;
using HG.TickManagers.Interfaces;
using HG.Types;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class ActorBulletManager : IVectoredTickManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public List<subType> VisibleOfType<subType>() where subType : BulletBase => _manager.VisibleOfType<subType>();
        public List<BulletBase> VisibleOfType() => _manager.VisibleOfType<BulletBase>();
        public List<subType> OfType<subType>() where subType : BulletBase => _manager.OfType<subType>();

        public ActorBulletManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            foreach (var bullet in VisibleOfType<BulletBase>())
            {
                bullet.ApplyMotion(displacementVector);

                //Check to see if the bullet hit the player:
                bullet.ApplyIntelligence(displacementVector, _core.Player.Actor);

                //Check to see if the bullet hit an enemy.
                foreach (var enemy in _manager.VisibleOfType<EnemyBase>())
                {
                    bullet.ApplyIntelligence(displacementVector, enemy);
                }

                foreach (var enemy in _manager.VisibleOfType<ActorAttachment>())
                {
                    bullet.ApplyIntelligence(displacementVector, enemy);
                }
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<BulletBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public BulletBase CreateLocked(WeaponBase weapon, ActorBase firedFrom, ActorBase lockedTarget, HgPoint<double> xyOffset = null)
        {
            lock (_manager.Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase Create(WeaponBase weapon, ActorBase firedFrom, HgPoint<double> xyOffset = null)
        {
            lock (_manager.Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(BulletBase obj)
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
