using Si.Game.Engine;
using Si.Game.Managers;
using Si.Game.Sprites;
using Si.Game.Sprites.Enemies.Bosses.BasesAndInterfaces;
using Si.Game.Sprites.Enemies.Peons.BasesAndInterfaces;
using Si.Game.TickControllers.BasesAndInterfaces;
using Si.Game.Weapons.BasesAndInterfaces;
using Si.Game.Weapons.Munitions;
using Si.Shared.Types.Geometry;
using System.Collections.Generic;

namespace Si.Game.Controller
{
    internal class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        public MunitionSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            var objectsThatCanBeHit = new List<_SpriteShipBase>
            {
                GameCore.Player.Sprite
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
        public bool TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<_SpriteShipBase> objectsThatCanBeHit, SiPoint displacementVector)
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

        public MunitionBase Create(WeaponBase weapon, SiPoint xyOffset = null)
        {
            lock (SpriteManager.Collection)
            {
                var obj = weapon.CreateMunition(xyOffset, null);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public MunitionBase CreateLocked(WeaponBase weapon, SpriteBase lockedTarget, SiPoint xyOffset = null)
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
