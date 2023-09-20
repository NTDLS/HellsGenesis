using HellsGenesis.Weapons.Projectiles;
using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Managers;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies.Bosses;
using NebulaSiege.Sprites.Enemies.Peons;
using NebulaSiege.TickControllers;
using NebulaSiege.Weapons;
using System.Collections.Generic;

namespace NebulaSiege.Controller
{
    internal class ProjectileSpriteTickController : _SpriteTickControllerBase<_ProjectileBase>
    {
        public ProjectileSpriteTickController(EngineCore core, EngineSpriteManager manager)
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

            foreach (var projectile in VisibleOfType<_ProjectileBase>())
            {
                projectile.ApplyMotion(displacementVector); //Move the projectile.

                if (TestObjectCollisionsAlongProjectilePath(projectile, objectsThatCanBeHit, displacementVector))
                {
                    projectile.Explode();
                }

                projectile.ApplyIntelligence(displacementVector);
            }
        }

        /// <summary>
        /// Takes the position of a projectile object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public bool TestObjectCollisionsAlongProjectilePath(_ProjectileBase projectile, List<_SpriteShipBase> objectsThatCanBeHit, NsPoint displacementVector)
        {
            var hitTestPosition = projectile.Location.ToWriteableCopy(); //Grab the new location of the projectile.

            //Loop backwards and hit-test each position along the projectiles path.
            for (int i = 0; i < projectile.Velocity.MaxSpeed; i++)
            {
                hitTestPosition.X -= projectile.Velocity.Angle.X;
                hitTestPosition.Y -= projectile.Velocity.Angle.Y;

                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.TryProjectileHit(displacementVector, projectile, hitTestPosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public _ProjectileBase Create(_WeaponBase weapon, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateProjectile(xyOffset, null);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public _ProjectileBase CreateLocked(_WeaponBase weapon, _SpriteBase lockedTarget, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateProjectile(xyOffset, lockedTarget);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
