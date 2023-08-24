using HG.Actors.Objects;
using HG.Actors.Objects.Enemies;
using HG.Actors.Objects.Weapons;
using HG.Actors.Objects.Weapons.Bullets;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;

namespace HG.Actors.Factories
{
    internal class EngineActorBulletManager
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorBulletManager(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
        }

        public void ExecuteWorldClockTick(HGPoint<double> appliedOffset)
        {
            foreach (var bullet in _manager.VisibleOfType<BulletBase>())
            {
                bullet.ApplyMotion(appliedOffset);

                //Check to see if the bullet hit the player:
                bullet.ApplyIntelligence(appliedOffset, _core.Player.Actor);

                //Check to see if the bullet hit an enemy.
                foreach (var enemy in _manager.VisibleOfType<EnemyBase>())
                {
                    bullet.ApplyIntelligence(appliedOffset, enemy);
                }

                foreach (var enemy in _manager.VisibleOfType<ActorAttachment>())
                {
                    bullet.ApplyIntelligence(appliedOffset, enemy);
                }
            }
        }

        public void DeleteAll()
        {
            lock (_manager.Collection)
            {
                _manager.OfType<BulletBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public BulletBase CreateLocked(WeaponBase weapon, ActorBase firedFrom, ActorBase lockedTarget, HGPoint<double> xyOffset = null)
        {
            lock (_manager.Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                _manager.Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase Create(WeaponBase weapon, ActorBase firedFrom, HGPoint<double> xyOffset = null)
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
    }
}
