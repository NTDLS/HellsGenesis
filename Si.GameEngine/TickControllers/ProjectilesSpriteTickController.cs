using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites.Enemies.Bosses.BasesAndInterfaces;
using Si.GameEngine.Sprites.Enemies.Peons.BasesAndInterfaces;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.GameEngine.Weapons.BasesAndInterfaces;
using Si.GameEngine.Weapons.Munitions;
using Si.Shared.Types.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Controller
{
    public class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        public MunitionSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            var munitions = VisibleOfType<MunitionBase>();
            if (munitions.Any())
            {
                var objectsThatCanBeHit = new List<SpriteShipBase>
                {
                    GameCore.Player.Sprite
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
