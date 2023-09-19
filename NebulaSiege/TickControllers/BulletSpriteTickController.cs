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
            var thingsThatCanBeHit = new List<_SpriteShipBase>
            {
                Core.Player.Sprite
            };

            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyBossBase>());
            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyPeonBase>());
            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

            foreach (var bullet in VisibleOfType<_BulletBase>())
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
