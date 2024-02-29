using NTDLS.DelegateThreadPooling;
using Si.GameEngine.ResourceManagers;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies.Bosses._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.TickControllers
{
    public class MunitionSpriteTickController : SpriteTickControllerBase<MunitionBase>
    {
        private readonly DelegateThreadPool _munitionTraversalThreadPool;

        public MunitionSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _munitionTraversalThreadPool = new(gameEngine.Settings.MunitionTraversalThreads);

            gameEngine.OnStopEngine += (sender) =>
            {
                _munitionTraversalThreadPool.Stop();
            };
        }

        private struct HitObject
        {
            public SpriteShipBase Object { get; set; }
            public MunitionBase Munition { get; set; }

            public HitObject(MunitionBase munition, SpriteShipBase obj)
            {
                Object = obj;
                Munition = munition;
            }
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
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
                    munition.ApplyMotion(epoch, displacementVector); //Move the munition.
                    munition.ApplyIntelligence(epoch, displacementVector);

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
            var hitTestPosition = new SiVector(munition.Location - munition.Velocity.Angle * munition.Velocity.Speed);

            //Hit-test each position along the munitions path.
            for (int i = 0; i < munition.Velocity.Speed; i++)
            {
                hitTestPosition += munition.Velocity.Angle;

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

        public MunitionBase Create(WeaponBase weapon, SiVector xyOffset = null)
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
        public MunitionBase CreateLockedOnTo(WeaponBase weapon, SpriteBase lockedTarget, SiVector xyOffset = null)
        {
            var obj = weapon.CreateMunition(xyOffset, lockedTarget);
            SpriteManager.Add(obj);
            return obj;
        }
    }
}
