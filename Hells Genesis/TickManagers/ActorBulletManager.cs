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
        public List<BulletBase> Visible() => _manager.VisibleOfType<BulletBase>();
        public List<subType> OfType<subType>() where subType : BulletBase => _manager.OfType<subType>();

        public ActorBulletManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HgPoint<double> displacementVector)
        {
            var thingsThatCanBeHit = new List<dynamic>
            {
                _core.Player.Actor
            };

            thingsThatCanBeHit.AddRange(_manager.VisibleOfType<EnemyBossBase>());
            thingsThatCanBeHit.AddRange(_manager.VisibleOfType<EnemyPeonBase>());
            thingsThatCanBeHit.AddRange(_manager.VisibleOfType<ActorAttachment>());

            foreach (var bullet in VisibleOfType<BulletBase>())
            {
                bullet.ApplyMotion(displacementVector); //Move the bullet.

                foreach (var thing in thingsThatCanBeHit)
                {
                    bullet.ApplyIntelligence(displacementVector, thing);
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
