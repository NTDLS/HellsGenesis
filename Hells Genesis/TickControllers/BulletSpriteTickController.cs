using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites;
using HG.Sprites.Enemies.Bosses;
using HG.Sprites.Enemies.Peons;
using HG.TickControllers;
using HG.Weapons;
using HG.Weapons.Bullets;
using System.Collections.Generic;

namespace HG.Controller
{
    internal class BulletSpriteTickController : VectoredTickControllerBase<BulletBase>
    {
        public BulletSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            var thingsThatCanBeHit = new List<SpriteShipBase>
            {
                Core.Player.Sprite
            };

            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyBossBase>());
            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyPeonBase>());
            thingsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

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

        public BulletBase Create(WeaponBase weapon, SpriteBase firedFrom, HgPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase CreateLocked(WeaponBase weapon, SpriteBase firedFrom, SpriteBase lockedTarget, HgPoint xyOffset = null)
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
