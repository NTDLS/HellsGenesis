using HellsGenesis.Weapons.Munitions;
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
    internal class MunitionSpriteTickController : _SpriteTickControllerBase<_MunitionBase>
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

            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyBossBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<_SpriteEnemyPeonBase>());
            objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

            foreach (var munition in VisibleOfType<_MunitionBase>())
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
        public bool TestObjectCollisionsAlongMunitionPath(_MunitionBase munition, List<_SpriteShipBase> objectsThatCanBeHit, NsPoint displacementVector)
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

        public _MunitionBase Create(_WeaponBase weapon, NsPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateMunition(xyOffset, null);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public _MunitionBase CreateLocked(_WeaponBase weapon, _SpriteBase lockedTarget, NsPoint xyOffset = null)
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
