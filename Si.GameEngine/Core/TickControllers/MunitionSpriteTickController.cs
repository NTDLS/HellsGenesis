using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies.Bosses._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Shared.Types.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        public MunitionSpriteTickController(Engine gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            var munitions = VisibleOfType<MunitionBase>();
            if (munitions.Any())
            {
                var objectsThatCanBeHit = new List<SpriteShipBase>
                {
                    GameEngine.Player.Sprite
                };

                objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyBossBase>());
                objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteEnemyPeonBase>());
                objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpritePlayerBase>());
                objectsThatCanBeHit.AddRange(SpriteManager.VisibleOfType<SpriteAttachment>());

                foreach (var munition in munitions)
                {
                    munition.ApplyMotion(displacementVector); //Move the munition.

                    if (TestObjectCollisionsAlongMunitionPath(munition, objectsThatCanBeHit, displacementVector))
                    {
                        munition.Explode();
                    }

                    munition.ApplyIntelligence(displacementVector);
                }
            }
        }

        /// <summary>
        /// Takes the position of a munition object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public bool TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<SpriteShipBase> objectsThatCanBeHit, SiPoint displacementVector)
        {
            var hitTestPosition = munition.Location; //Grab the new location of the munition.

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
            var obj = weapon.CreateMunition(xyOffset, null);
            SpriteManager.Add(obj);
            return obj;
        }

        /// <summary>
        /// Creates a munition that is locked on to another sprite.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="lockedTarget"></param>
        /// <param name="xyOffset"></param>
        /// <returns></returns>
        public MunitionBase CreateLockedOnTo(WeaponBase weapon, SpriteBase lockedTarget, SiPoint xyOffset = null)
        {
            var obj = weapon.CreateMunition(xyOffset, lockedTarget);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
