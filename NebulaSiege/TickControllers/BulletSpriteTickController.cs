using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Managers;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies.Bosses;
using NebulaSiege.Sprites.Enemies.Peons;
using NebulaSiege.TickControllers;
using NebulaSiege.Weapons;
using NebulaSiege.Weapons.Bullets;
using System.Collections.Generic;

namespace NebulaSiege.Controller
{
    internal class BulletSpriteTickController : _SpriteTickControllerBase<_BulletBase>
    {
        public BulletSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(NsPoint displacementVector)
        {
            var objectsThatCanBeHit = new List<_SpriteShipBase>
            {
                Core.Player.Sprite
            };

            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyBossBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyPeonBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

            foreach (var bullet in VisibleOfType<_BulletBase>())
            {
                bullet.ApplyMotion(displacementVector); //Move the bullet.

                if (TestObjectCollisionsAlongBulletPath(bullet, objectsThatCanBeHit, displacementVector))
                {
                    bullet.Explode();
                }

                bullet.ApplyIntelligence(displacementVector);
            }
        }

        /// <summary>
        /// Takes the position of a bullet object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public bool TestObjectCollisionsAlongBulletPath(_BulletBase bullet, List<_SpriteShipBase> objectsThatCanBeHit, NsPoint displacementVector)
        {
            var hitTestPosition = bullet.Location.ToWriteableCopy(); //Grab the new location of the bullet.

            //Loop backwards and hit-test each position along the bullets path.
            for (int i = 0; i < bullet.Velocity.MaxSpeed; i++)
            {
                hitTestPosition.X -= bullet.Velocity.Angle.X;
                hitTestPosition.Y -= bullet.Velocity.Angle.Y;

                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.TryBulletHit(displacementVector, bullet, hitTestPosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public _BulletBase Create(_WeaponBase weapon, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public _BulletBase CreateLocked(_WeaponBase weapon, _SpriteBase lockedTarget, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
