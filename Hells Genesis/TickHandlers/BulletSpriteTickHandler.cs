using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.Sprites.Enemies.Bosses;
using HG.Sprites.Enemies.Peons;
using HG.TickHandlers.Interfaces;
using HG.Weapons;
using HG.Weapons.Bullets;
using System.Collections.Generic;

namespace HG.TickHandlers
{
    internal class BulletSpriteTickHandler : IVectoredTickManager
    {
        private readonly EngineCore _core;
        private readonly EngineSpriteManager _controller;

        public List<subType> VisibleOfType<subType>() where subType : BulletBase => _controller.VisibleOfType<subType>();
        public List<BulletBase> Visible() => _controller.VisibleOfType<BulletBase>();
        public List<subType> OfType<subType>() where subType : BulletBase => _controller.OfType<subType>();

        public BulletSpriteTickHandler(EngineCore core, EngineSpriteManager manager)
        {
            _core = core;
            _controller = manager;
        }

        public void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            var thingsThatCanBeHit = new List<SpriteShipBase>
            {
                _core.Player.Sprite
            };

            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<SpriteEnemyBossBase>());
            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<SpriteEnemyPeonBase>());
            thingsThatCanBeHit.AddRange(_controller.VisibleOfType<SpriteAttachment>());

            foreach (var bullet in VisibleOfType<BulletBase>())
            {
                bullet.ApplyMotion(displacementVector); //Move the bullet.

                var hitTestPosition = bullet.Location.ToWriteableCopy(); //Grab the new location of the bullet.

                //Loop backwards and hit-test each position along the bullets path.
                for (int i = 0; i < bullet.Velocity.MaxSpeed; i++)
                {
                    hitTestPosition.X -= bullet.Velocity.Angle.X;
                    hitTestPosition.Y -= bullet.Velocity.Angle.Y;

                    foreach (var thing in thingsThatCanBeHit)
                    {
                        if (thing.TestHit(displacementVector, bullet, hitTestPosition))
                        {
                            bullet.Explode();
                            break;
                        }
                    }
                }

                bullet.ApplyIntelligence(displacementVector);
            }
        }

        #region Factories.

        public void DeleteAll()
        {
            lock (_controller.Collection)
            {
                _controller.OfType<BulletBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public BulletBase CreateLocked(WeaponBase weapon, SpriteBase firedFrom, SpriteBase lockedTarget, HgPoint xyOffset = null)
        {
            lock (_controller.Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase Create(WeaponBase weapon, SpriteBase firedFrom, HgPoint xyOffset = null)
        {
            lock (_controller.Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                _controller.Collection.Add(obj);
                return obj;
            }
        }

        public void Delete(BulletBase obj)
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
