using NTDLS.DelegateThreadPooling;
using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies.Bosses._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        private readonly DelegateThreadPool _munitionTraversalThreadPool;

        public MunitionSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _munitionTraversalThreadPool = new(gameEngine.Settings.MunitionTraversalThreads);

            gameEngine.OnStopEngine += (GameEngineCore sender) =>
            {
                _munitionTraversalThreadPool.Stop();
            };
        }

        private class HitObject
        {
            public SpriteShipBase Object { get; set; }
            public MunitionBase Munition { get; set; }

            public HitObject(MunitionBase munition, SpriteShipBase obj)
            {
                Object = obj;
                Munition = munition;
            }
        }

        public override void ExecuteWorldClockTick(double epochMilliseconds, SiPoint displacementVector)
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

                //Create a collection of threads so we can wait on the ones that we start.
                var threadCollection = _munitionTraversalThreadPool.CreateQueueStateCollection();

                List<HitObject> hitObjects = new();

                foreach (var munition in munitions)
                {
                    munition.ApplyMotion(epochMilliseconds, displacementVector); //Move the munition.
                    munition.ApplyIntelligence(epochMilliseconds, displacementVector);

                    if (munition.IsDeadOrExploded == false)
                    {
                        //Enqueue an item into the thread pool.
                        threadCollection.Enqueue(() =>
                        {
                            var hitObject = TestObjectCollisionsAlongMunitionPath(munition, objectsThatCanBeHit);
                            if (hitObject != null)
                            {
                                lock (hitObjects)
                                {
                                    hitObjects.Add(new(munition, hitObject));
                                }
                            }
                        });
                    }
                }

                //Wait on all enqueued threads to complete.
                if (SiUtility.TryAndIgnore(() => threadCollection.WaitForCompletion()) == false)
                {
                    return;
                }

                //Take actions with the munitions that hit objects.
                foreach (var hitObject in hitObjects)
                {
                    if (hitObject.Object.IsDeadOrExploded == false)
                    {
                        hitObject.Munition.Explode();
                        hitObject.Object.MunitionHit(hitObject.Munition);
                    }
                }
            }
        }

        /// <summary>
        /// Takes the position of a munition object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public SpriteShipBase TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<SpriteShipBase> objectsThatCanBeHit)
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
                    if (obj.TryMunitionHit(munition, hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
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
