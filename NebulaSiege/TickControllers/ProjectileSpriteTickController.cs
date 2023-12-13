using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Managers;
using NebulaSiege.Sprites;
using NebulaSiege.Sprites.Enemies.Bosses.BaseClasses;
using NebulaSiege.Sprites.Enemies.Peons.BaseClasses;
using NebulaSiege.TickControllers.BaseClasses;
using NebulaSiege.Weapons.BaseClasses;
using NebulaSiege.Weapons.Munitions;
using System.Collections.Generic;

namespace NebulaSiege.Controller
{
    internal class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        public MunitionSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(NsPoint displacementVector)
        {
            var objectsThatCanBeHit = new List<_SpriteShipBase>
            {
                Core.Player.Sprite
            };

            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyBossBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyPeonBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

            foreach (var munition in VisibleOfType<MunitionBase>())
            {
                munition.ApplyMotion(displacementVector); //Move the munition.

                if (TestObjectCollisionsAlongMunitionPath(munition, objectsThatCanBeHit, displacementVector))
                {
                    munition.Explode();
                }

                munition.ApplyIntelligence(displacementVector);
            }
        }

        /// <summary>
        /// Takes the position of a munition object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public bool TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<_SpriteShipBase> objectsThatCanBeHit, NsPoint displacementVector)
        {
            var hitTestPosition = munition.Location.ToWriteableCopy(); //Grab the new location of the munition.

            //Loop backwards and hit-test each position along the munitions path.
            for (int i = 0; i < munition.Velocity.MaxSpeed; i++)
            {
                hitTestPosition.X -= munition.Velocity.Angle.X;
                hitTestPosition.Y -= munition.Velocity.Angle.Y;

                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.TryMunitionHit(displacementVector, munition, hitTestPosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public MunitionBase Create(WeaponBase weapon, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateMunition(xyOffset, null);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public MunitionBase CreateLocked(WeaponBase weapon, SpriteBase lockedTarget, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateMunition(xyOffset, lockedTarget);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
