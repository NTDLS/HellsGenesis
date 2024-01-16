using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.ThreadPooling;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies.Bosses._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.Types.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Si.GameEngine.Core.TickControllers
{
    public class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        public MunitionSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        //private readonly DelegateThreadPool _hitTestPool = new(10);

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

                //var threadCollection = _hitTestPool.CreateCollection();

                foreach (var munition in munitions)
                {
                    //threadCollection.Enqueue(munition, () =>
                    //{
                        munition.ApplyMotion(displacementVector); //Move the munition.

                        if (TestObjectCollisionsAlongMunitionPath(munition, objectsThatCanBeHit, displacementVector))
                        {
                            munition.Explode();
                        }

                        munition.ApplyIntelligence(displacementVector);
                    //});
                }

                //threadCollection.WaitAll();
            }
        }

        /// <summary>
        /// Takes the position of a munition object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public bool TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<SpriteShipBase> objectsThatCanBeHit, SiPoint displacementVector)
        {
            //Reverse the munition to its starting position.
            var hitTestPosition = new SiPoint
            {
                X = munition.Location.X - munition.Velocity.MaxSpeed * munition.Velocity.Angle.X,
                Y = munition.Location.Y - munition.Velocity.MaxSpeed * munition.Velocity.Angle.Y
            };

            //Hit-test each position along the munitions path.
            for (int i = 0; i < munition.Velocity.MaxSpeed; i++)
            {
                hitTestPosition.X += munition.Velocity.Angle.X;
                hitTestPosition.Y += munition.Velocity.Angle.Y;

                foreach (var obj in objectsThatCanBeHit)
                {

                    if (obj.IsMunitionHit(munition, hitTestPosition))
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
