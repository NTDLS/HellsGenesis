using HG.Actors.Objects;
using HG.Actors.Objects.Weapons;
using HG.Actors.Objects.Weapons.Bullets;
using HG.Engine;
using HG.Engine.Managers;
using HG.Types;

namespace HG.Actors.Factories
{
    internal class EngineActorBulletFactory
    {
        private readonly Core _core;
        private readonly EngineActorManager _manager;

        public EngineActorBulletFactory(Core core, EngineActorManager manager)
        {
            _core = core;
            _manager = manager;
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
